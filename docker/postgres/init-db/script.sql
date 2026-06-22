/* 
 * ANIMAL SHELTER DATABASE - PHASE 1
 * Extension: pgcrypto for UUID generation
 * Conventions: snake_case, English naming, Soft Delete, Audit Logs
 */

-- 1. EXTENSIONS
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 2. ENUM TYPES
CREATE TYPE species_enum AS ENUM ('cat', 'dog');
CREATE TYPE sex_enum AS ENUM ('male', 'female', 'unknown');
CREATE TYPE animal_status_enum AS ENUM ('shelter', 'owner', 'fostered', 'adopted', 'dead');
CREATE TYPE compat_type_enum AS ENUM ('cat', 'dog', 'young_child', 'child', 'garden', 'pony');
CREATE TYPE compat_value_enum AS ENUM ('yes', 'no', 'not_tested');
CREATE TYPE intake_reason_enum AS ENUM ('abandonment', 'stray', 'owner_death', 'seizure', 'return');
CREATE TYPE exit_reason_enum AS ENUM ('adoption', 'owner_return', 'death');
CREATE TYPE adoption_status_enum AS ENUM ('requested', 'approved', 'env_rejected', 'behaviour_rejected');

-- 3. SEQUENCE & FUNCTION FOR CUSTOM ANIMAL ID (yymmdd99999)
-- The sequence cycles every 100k entries to reset for the next day/period
CREATE SEQUENCE seq_animal_id_suffix START 1 MAXVALUE 99999 CYCLE;

CREATE OR REPLACE FUNCTION fn_generate_animal_id() 
RETURNS VARCHAR(11) AS $$
BEGIN
    RETURN TO_CHAR(CURRENT_DATE, 'YYMMDD') || LPAD(nextval('seq_animal_id_suffix')::TEXT, 5, '0');
END;
$$ LANGUAGE plpgsql;

-- 4. AUDIT TRIGGER FUNCTION
CREATE OR REPLACE FUNCTION fn_update_updated_at() 
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 5. TABLES

-- Address management (Shared by contacts)
CREATE TABLE addresses (
    id_address UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    street VARCHAR(255) NOT NULL,
    number VARCHAR(20) NOT NULL,
    box VARCHAR(20),
    post_code VARCHAR(20) NOT NULL,
    city VARCHAR(150) NOT NULL,
    country VARCHAR(100) DEFAULT 'Belgium',
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ
);

-- Main Animal table
CREATE TABLE animals (
    id_animal VARCHAR(11) PRIMARY KEY DEFAULT fn_generate_animal_id(),
    name VARCHAR(100) NOT NULL,
    species species_enum NOT NULL,
    sex sex_enum NOT NULL,
    colors VARCHAR(100),
    is_sterilised BOOLEAN DEFAULT FALSE,
    sterilisation_date DATE,
    birth_date DATE,
    death_date DATE,
    description TEXT,
    particularities TEXT,
    current_status animal_status_enum DEFAULT 'shelter',
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT check_id_format CHECK (id_animal ~ '^[0-9]{11}$')
);

-- Contact persons with Role Bitmask (1:Volunteer, 2:Adopter, 4:Candidate, 8:Other)
CREATE TABLE contacts (
    id_person UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_address UUID REFERENCES addresses(id_address),
    last_name VARCHAR(150) NOT NULL,
    first_name VARCHAR(150) NOT NULL,
    national_register_encrypted BYTEA, -- Encrypted at BLL level
    national_register_hash BYTEA, -- Hash for unicity
    gsm VARCHAR(50),
    phone VARCHAR(50),
    email VARCHAR(255),
    role_flags SMALLINT DEFAULT 0, 
    rgpd_consent_date TIMESTAMPTZ,
    is_anonymised BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT uq_contacts_national_register_hash UNIQUE (national_register_hash)
);

-- Compatibility traits (One row per type per animal)
CREATE TABLE compatibilities (
    id_compatibility UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal) ON DELETE CASCADE,
    target_type compat_type_enum NOT NULL,
    value compat_value_enum NOT NULL,
    description TEXT,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    UNIQUE(id_animal, target_type)
);

-- Vaccination tracking
CREATE TABLE vaccinations (
    id_vaccin UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal),
    vaccine_name VARCHAR(150) NOT NULL,
    vaccine_date DATE NOT NULL,
    is_done BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ
);

-- History of arrivals at the shelter
CREATE TABLE intake_histories (
    id_intake UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal),
    id_person UUID REFERENCES contacts(id_person),
    intake_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reason intake_reason_enum NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    -- Contact is mandatory for specific reasons (Abandonment, Seizure, Return)
    CONSTRAINT check_intake_person CHECK (
        (reason IN ('abandonment', 'seizure', 'return') AND id_person IS NOT NULL) OR 
        (reason IN ('stray', 'owner_death'))
    )
);

-- History of departures
CREATE TABLE exit_histories (
    id_exit UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal),
    id_person UUID REFERENCES contacts(id_person),
    exit_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reason exit_reason_enum NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    -- Contact is mandatory for Adoption or Owner Return
    CONSTRAINT check_exit_person CHECK (
        (reason IN ('adoption', 'owner_return') AND id_person IS NOT NULL) OR 
        (reason = 'death')
    )
);

-- Tracking foster family stays
CREATE TABLE foster_stays (
    id_foster UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal),
    id_person UUID REFERENCES contacts(id_person),
    start_date DATE NOT NULL,
    end_date DATE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT check_foster_dates CHECK (end_date IS NULL OR end_date >= start_date)
);

-- Adoption application process
CREATE TABLE adoption_files (
    id_adoption UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_animal VARCHAR(11) REFERENCES animals(id_animal),
    id_person UUID REFERENCES contacts(id_person),
    request_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    status adoption_status_enum DEFAULT 'requested',
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ
);

-- 6. DYNAMIC TRIGGER APPLICATION
-- Automatically applies the update_at trigger to every table in the public schema
DO $$ 
DECLARE 
    t text;
BEGIN
    FOR t IN SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
    LOOP
        EXECUTE format('CREATE TRIGGER trg_update_%I BEFORE UPDATE ON %I FOR EACH ROW EXECUTE FUNCTION fn_update_updated_at()', t, t);
    END LOOP;
END $$;

-- 7. GLOBAL SUMMARY VIEW
CREATE VIEW v_animal_summary AS
SELECT 
    a.id_animal, 
    a.name, 
    a.species, 
    a.current_status,
    (SELECT intake_date FROM intake_histories WHERE id_animal = a.id_animal ORDER BY intake_date DESC LIMIT 1) as last_entry_date
FROM animals a
WHERE a.deleted_at IS NULL;

/*
 * ANIMAL SHELTER DATABASE - PHASE 2
 * Stored Procedures
 */
-- ANIMALS

CREATE OR REPLACE FUNCTION sp_animal_insert(
    p_name            VARCHAR,
    p_species         species_enum,
    p_sex             sex_enum,
    p_colors          VARCHAR,
    p_is_sterilised   BOOLEAN,
    p_sterilisation_date DATE,
    p_birth_date      DATE,
    p_description     TEXT,
    p_particularities TEXT
) RETURNS VARCHAR AS $$
DECLARE
    v_id VARCHAR(11);
BEGIN
    INSERT INTO animals (name, species, sex, colors, is_sterilised, sterilisation_date, birth_date, description, particularities)
    VALUES (p_name, p_species, p_sex, p_colors, p_is_sterilised, p_sterilisation_date, p_birth_date, p_description, p_particularities)
    RETURNING id_animal INTO v_id;
    RETURN v_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_animal_get_by_id(p_id VARCHAR)
RETURNS SETOF animals AS $$
    SELECT * FROM animals WHERE id_animal = p_id AND deleted_at IS NULL;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_animal_get_all_active()
RETURNS SETOF animals AS $$
    SELECT * FROM animals WHERE deleted_at IS NULL ORDER BY created_at DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_animal_update(
    p_id              VARCHAR,
    p_name            VARCHAR,
    p_colors          VARCHAR,
    p_description     TEXT,
    p_particularities TEXT,
    p_status          animal_status_enum
) RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE animals SET
        name              = p_name,
        colors            = p_colors,
        description       = p_description,
        particularities   = p_particularities,
        current_status    = p_status
    WHERE id_animal = p_id;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_animal_soft_delete(p_id VARCHAR)
RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE animals SET deleted_at = CURRENT_TIMESTAMP WHERE id_animal = p_id;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

-- ADDRESSES

CREATE OR REPLACE FUNCTION sp_address_get_by_id(p_id UUID)
RETURNS SETOF addresses AS $$
    SELECT * FROM addresses WHERE id_address = p_id AND deleted_at IS NULL;
$$ LANGUAGE sql STABLE;

-- CONTACTS
-- (adresse + contact gérés ensemble dans une seule procédure)
CREATE OR REPLACE FUNCTION sp_contact_register(
    p_street       VARCHAR,
    p_number       VARCHAR,
    p_box          VARCHAR,
    p_post_code    VARCHAR,
    p_city         VARCHAR,
    p_country      VARCHAR,
    p_last_name    VARCHAR,
    p_first_name   VARCHAR,
    p_nr_encrypted BYTEA,
    p_nr_hash      BYTEA,
    p_gsm          VARCHAR,
    p_phone        VARCHAR,
    p_email        VARCHAR,
    p_role_flags   SMALLINT,
    p_rgpd_date    TIMESTAMPTZ
) RETURNS UUID AS $$
DECLARE
    v_address_id UUID := NULL;
    v_contact_id UUID;
BEGIN
    IF p_street IS NOT NULL THEN
        INSERT INTO addresses (street, number, box, post_code, city, country)
        VALUES (p_street, p_number, p_box, p_post_code, p_city, p_country)
        RETURNING id_address INTO v_address_id;
    END IF;

    INSERT INTO contacts (id_address, last_name, first_name, national_register_encrypted, national_register_hash, gsm, phone, email, role_flags, rgpd_consent_date)
    VALUES (v_address_id, p_last_name, p_first_name, p_nr_encrypted, p_nr_hash, p_gsm, p_phone, p_email, p_role_flags, p_rgpd_date)
    RETURNING id_person INTO v_contact_id;

    RETURN v_contact_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_contact_get_by_id(p_id UUID)
RETURNS SETOF contacts AS $$
    SELECT * FROM contacts WHERE id_person = p_id AND deleted_at IS NULL;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_contact_get_all()
RETURNS SETOF contacts AS $$
    SELECT * FROM contacts WHERE deleted_at IS NULL ORDER BY last_name, first_name;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_contact_update_full(
    p_contact_id UUID,
    p_last_name  VARCHAR,
    p_first_name VARCHAR,
    p_gsm        VARCHAR,
    p_phone      VARCHAR,
    p_email      VARCHAR,
    p_role_flags SMALLINT,
    p_id_address UUID,
    p_street     VARCHAR,
    p_number     VARCHAR,
    p_box        VARCHAR,
    p_post_code  VARCHAR,
    p_city       VARCHAR,
    p_country    VARCHAR
) RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    IF p_id_address IS NULL THEN
        SELECT id_address INTO p_id_address
        FROM contacts
        WHERE id_person = p_contact_id;
    END IF;
    IF p_id_address IS NOT NULL THEN
        UPDATE addresses SET
            street    = p_street,
            number    = p_number,
            box       = p_box,
            post_code = p_post_code,
            city      = p_city,
            country   = p_country
        WHERE id_address = p_id_address;
    END IF;

    UPDATE contacts SET
        last_name  = p_last_name,
        first_name = p_first_name,
        gsm        = p_gsm,
        phone      = p_phone,
        email      = p_email,
        role_flags = p_role_flags
    WHERE id_person = p_contact_id;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_contact_soft_delete(p_id UUID)
RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE contacts SET deleted_at = CURRENT_TIMESTAMP WHERE id_person = p_id;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

-- VACCINATIONS
CREATE OR REPLACE FUNCTION sp_vaccination_insert(
    p_id_animal VARCHAR,
    p_name      VARCHAR,
    p_date      DATE,
    p_is_done   BOOLEAN
) RETURNS UUID AS $$
DECLARE
    v_id UUID;
BEGIN
    INSERT INTO vaccinations (id_animal, vaccine_name, vaccine_date, is_done)
    VALUES (p_id_animal, p_name, p_date, p_is_done)
    RETURNING id_vaccin INTO v_id;
    RETURN v_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_vaccination_get_by_animal(p_id_animal VARCHAR)
RETURNS SETOF vaccinations AS $$
    SELECT * FROM vaccinations
    WHERE id_animal = p_id_animal AND deleted_at IS NULL
    ORDER BY vaccine_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_vaccination_update(
    p_id      UUID,
    p_name    VARCHAR,
    p_date    DATE,
    p_is_done BOOLEAN
) RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE vaccinations SET
        vaccine_name = p_name,
        vaccine_date = p_date,
        is_done      = p_is_done,
        updated_at   = NOW()
    WHERE id_vaccin = p_id AND deleted_at IS NULL;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_vaccination_soft_delete(p_id UUID)
RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE vaccinations SET deleted_at = NOW(), updated_at = NOW()
    WHERE id_vaccin = p_id AND deleted_at IS NULL;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

-- COMPATIBILITIES
CREATE OR REPLACE FUNCTION sp_compatibility_upsert(
    p_id_animal VARCHAR,
    p_type      compat_type_enum,
    p_value     compat_value_enum,
    p_desc      TEXT
) RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    INSERT INTO compatibilities (id_animal, target_type, value, description)
    VALUES (p_id_animal, p_type, p_value, p_desc)
    ON CONFLICT (id_animal, target_type)
    DO UPDATE SET value = p_value, description = p_desc, deleted_at = NULL;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_compatibility_get_by_animal(p_id_animal VARCHAR)
RETURNS SETOF compatibilities AS $$
    SELECT * FROM compatibilities WHERE id_animal = p_id_animal AND deleted_at IS NULL;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_compatibility_soft_delete(
    p_id_animal VARCHAR,
    p_type      compat_type_enum
) RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE compatibilities SET deleted_at = CURRENT_TIMESTAMP
    WHERE id_animal = p_id_animal AND target_type = p_type;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

-- FOSTER STAYS
CREATE OR REPLACE FUNCTION sp_foster_insert(
    p_id_animal VARCHAR,
    p_id_person UUID,
    p_start_date DATE
) RETURNS UUID AS $$
DECLARE
    v_id UUID;
BEGIN
    INSERT INTO foster_stays (id_animal, id_person, start_date)
    VALUES (p_id_animal, p_id_person, p_start_date)
    RETURNING id_foster INTO v_id;
    RETURN v_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_foster_get_by_animal(p_id_animal VARCHAR)
RETURNS TABLE(
    id_foster   UUID,
    id_animal   VARCHAR,
    id_person   UUID,
    start_date  DATE,
    end_date    DATE,
    animal_name VARCHAR,
    first_name  VARCHAR,
    last_name   VARCHAR
) AS $$
    SELECT f.id_foster, f.id_animal, f.id_person, f.start_date, f.end_date,
           a.name AS animal_name, c.first_name, c.last_name
    FROM foster_stays f
    JOIN animals  a ON f.id_animal = a.id_animal
    JOIN contacts c ON f.id_person = c.id_person
    WHERE f.id_animal = p_id_animal
    ORDER BY f.start_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_foster_get_active_by_contact(p_id_person UUID)
RETURNS TABLE(
    id_foster   UUID,
    id_animal   VARCHAR,
    id_person   UUID,
    start_date  DATE,
    end_date    DATE,
    animal_name VARCHAR,
    first_name  VARCHAR,
    last_name   VARCHAR
) AS $$
    SELECT f.id_foster, f.id_animal, f.id_person, f.start_date, f.end_date,
           a.name AS animal_name, c.first_name, c.last_name
    FROM foster_stays f
    JOIN animals  a ON f.id_animal = a.id_animal
    JOIN contacts c ON f.id_person = c.id_person
    WHERE f.id_person = p_id_person AND f.end_date IS NULL
    ORDER BY f.start_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_foster_get_history_by_contact(p_id_person UUID)
RETURNS TABLE(
    id_foster   UUID,
    id_animal   VARCHAR,
    id_person   UUID,
    start_date  DATE,
    end_date    DATE,
    animal_name VARCHAR,
    first_name  VARCHAR,
    last_name   VARCHAR
) AS $$
    SELECT f.id_foster, f.id_animal, f.id_person, f.start_date, f.end_date,
           a.name AS animal_name, c.first_name, c.last_name
    FROM foster_stays f
    JOIN animals  a ON f.id_animal = a.id_animal
    JOIN contacts c ON f.id_person = c.id_person
    WHERE f.id_person = p_id_person
    ORDER BY f.start_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_foster_end_stay(p_id_foster UUID, p_end_date DATE)
RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE foster_stays SET end_date = p_end_date WHERE id_foster = p_id_foster;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;

-- ADOPTION FILES
CREATE OR REPLACE FUNCTION sp_adoption_insert(
    p_id_animal VARCHAR,
    p_id_person UUID,
    p_status    adoption_status_enum
) RETURNS UUID AS $$
DECLARE
    v_id UUID;
BEGIN
    INSERT INTO adoption_files (id_animal, id_person, status)
    VALUES (p_id_animal, p_id_person, p_status)
    RETURNING id_adoption INTO v_id;
    RETURN v_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_adoption_get_all()
RETURNS TABLE(
    id_adoption  UUID,
    id_animal    VARCHAR,
    id_person    UUID,
    request_date TIMESTAMPTZ,
    status       adoption_status_enum,
    animal_name  VARCHAR,
    first_name   VARCHAR,
    last_name    VARCHAR
) AS $$
    SELECT ad.id_adoption, ad.id_animal, ad.id_person, ad.request_date, ad.status,
           a.name AS animal_name, c.first_name, c.last_name
    FROM adoption_files ad
    JOIN animals  a ON ad.id_animal = a.id_animal
    JOIN contacts c ON ad.id_person = c.id_person
    WHERE ad.deleted_at IS NULL
    ORDER BY ad.request_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_adoption_get_by_id(p_id UUID)
RETURNS TABLE(
    id_adoption  UUID,
    id_animal    VARCHAR,
    id_person    UUID,
    request_date TIMESTAMPTZ,
    status       adoption_status_enum,
    animal_name  VARCHAR,
    first_name   VARCHAR,
    last_name    VARCHAR
) AS $$
    SELECT ad.id_adoption, ad.id_animal, ad.id_person, ad.request_date, ad.status,
           a.name AS animal_name, c.first_name, c.last_name
    FROM adoption_files ad
    JOIN animals  a ON ad.id_animal = a.id_animal
    JOIN contacts c ON ad.id_person = c.id_person
    WHERE ad.id_adoption = p_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_adoption_get_by_animal(p_id_animal VARCHAR)
RETURNS TABLE(
    id_adoption  UUID,
    id_animal    VARCHAR,
    id_person    UUID,
    request_date TIMESTAMPTZ,
    status       adoption_status_enum,
    animal_name  VARCHAR,
    first_name   VARCHAR,
    last_name    VARCHAR
) AS $$
    SELECT ad.id_adoption, ad.id_animal, ad.id_person, ad.request_date, ad.status,
           a.name AS animal_name, c.first_name, c.last_name
    FROM adoption_files ad
    JOIN animals  a ON ad.id_animal = a.id_animal
    JOIN contacts c ON ad.id_person = c.id_person
    WHERE ad.id_animal = p_id_animal AND ad.deleted_at IS NULL
    ORDER BY ad.request_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_adoption_get_by_contact(p_id_person UUID)
RETURNS TABLE(
    id_adoption  UUID,
    id_animal    VARCHAR,
    id_person    UUID,
    request_date TIMESTAMPTZ,
    status       adoption_status_enum,
    animal_name  VARCHAR,
    first_name   VARCHAR,
    last_name    VARCHAR
) AS $$
    SELECT ad.id_adoption, ad.id_animal, ad.id_person, ad.request_date, ad.status,
           a.name AS animal_name, c.first_name, c.last_name
    FROM adoption_files ad
    JOIN animals  a ON ad.id_animal = a.id_animal
    JOIN contacts c ON ad.id_person = c.id_person
    WHERE ad.id_person = p_id_person AND ad.deleted_at IS NULL
    ORDER BY ad.request_date DESC;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION sp_adoption_update_status(p_id UUID, p_status adoption_status_enum)
RETURNS INTEGER AS $$
DECLARE
    v_rows INTEGER;
BEGIN
    UPDATE adoption_files SET status = p_status WHERE id_adoption = p_id;
    GET DIAGNOSTICS v_rows = ROW_COUNT;
    RETURN v_rows;
END;
$$ LANGUAGE plpgsql;