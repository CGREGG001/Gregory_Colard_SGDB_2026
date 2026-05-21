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
    gsm VARCHAR(50),
    phone VARCHAR(50),
    email VARCHAR(255),
    role_flags SMALLINT DEFAULT 0, 
    rgpd_consent_date TIMESTAMPTZ,
    is_anonymised BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ
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