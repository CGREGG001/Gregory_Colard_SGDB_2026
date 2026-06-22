/*
 * ANIMAL SHELTER DATABASE
 * Set datas for tables
*/
INSERT INTO addresses (id_address, street, number, post_code, city, country) VALUES
    ('a1000000-0000-0000-0000-000000000001', 'Rue de la Paix',      '12',   '1000', 'Bruxelles',  'Belgium'),
    ('a1000000-0000-0000-0000-000000000002', 'Avenue des Fleurs',   '34B',  '4000', 'Liège',      'Belgium'),
    ('a1000000-0000-0000-0000-000000000003', 'Chaussée de Namur',   '7',    '5000', 'Namur',      'Belgium'),
    ('a1000000-0000-0000-0000-000000000004', 'Rue du Moulin',       '89',   '1300', 'Wavre',      'Belgium'),
    ('a1000000-0000-0000-0000-000000000005', 'Boulevard du Roi',    '3',    '6000', 'Charleroi',  'Belgium');

-- CONTACTS
INSERT INTO contacts (id_person, id_address, last_name, first_name, gsm, email, role_flags, rgpd_consent_date) VALUES
    -- Bénévole + Candidat adoptant
    ('c1000000-0000-0000-0000-000000000001',
     'a1000000-0000-0000-0000-000000000001',
     'Dupont', 'Marie', '0475 12 34 56', 'marie.dupont@email.be', 5,
     '2024-01-15 00:00:00+01'),

    -- Bénévole
    ('c1000000-0000-0000-0000-000000000002',
     'a1000000-0000-0000-0000-000000000002',
     'Lecomte', 'Jean', '0496 78 90 12', 'jean.lecomte@email.be', 1,
     '2024-03-10 00:00:00+01'),

    -- Famille d'accueil (Bénévole)
    ('c1000000-0000-0000-0000-000000000003',
     'a1000000-0000-0000-0000-000000000003',
     'Martin', 'Sophie', '0478 55 66 77', 'sophie.martin@email.be', 1,
     '2023-11-20 00:00:00+01'),

    -- Adoptant
    ('c1000000-0000-0000-0000-000000000004',
     'a1000000-0000-0000-0000-000000000004',
     'Bernard', 'Pierre', '0468 11 22 33', 'pierre.bernard@email.be', 2,
     '2025-02-08 00:00:00+01'),

    -- Bénévole + Famille d'accueil
    ('c1000000-0000-0000-0000-000000000005',
     'a1000000-0000-0000-0000-000000000005',
     'Renard', 'Julie', '0492 44 55 66', 'julie.renard@email.be', 1,
     '2024-06-01 00:00:00+02');

-- ANIMAUX
INSERT INTO animals (id_animal, name, species, sex, colors, is_sterilised, sterilisation_date, birth_date, description, particularities, current_status) VALUES
    ('26010100001', 'Max',   'dog', 'male',    'Noir et feu',     TRUE,  '2022-05-10', '2020-03-15',
     'Berger belge de 5 ans, très affectueux et bien socialisé.',
     'Craint les feux d''artifice.', 'shelter'),

    ('26010100002', 'Luna',  'cat', 'female',  'Grise tigrée',    TRUE,  '2021-09-01', '2019-07-22',
     'Chatte calme et douce, idéale pour un foyer tranquille.',
     'Préfère être le seul animal à la maison.', 'shelter'),

    ('26010100003', 'Rex',   'dog', 'male',    'Doré',            FALSE, NULL,          '2022-11-03',
     'Labrador joueur et énergique, adore les enfants.',
     'A besoin de beaucoup d''exercice.', 'fostered'),

    ('26010100004', 'Mia',   'cat', 'female',  'Blanche et roux', TRUE,  '2023-02-14', '2021-04-10',
     'Petite chatte curieuse et joueuse.',
     NULL, 'shelter'),

    ('26010100005', 'Buddy', 'dog', 'male',    'Beige',           TRUE,  '2020-08-20', '2018-06-30',
     'Vieux golden retriever très doux, parfait compagnon pour une famille.',
     'Problèmes de hanches, nécessite des soins réguliers.', 'adopted'),

    ('26010100006', 'Nala',  'cat', 'female',  'Noire',           FALSE, NULL,          '2024-01-05',
     'Jeune chatte sauvage en cours de socialisation.',
     'Nécessite de la patience et un environnement calme.', 'shelter'),

    ('26010100007', 'Oscar', 'dog', 'male',    'Blanc et marron', TRUE,  '2022-03-18', '2021-09-12',
     'Beagle espiègle, très attachant.',
     'Tendance à fuguer, jardin clôturé indispensable.', 'shelter'),

    ('26010100008', 'Bella', 'cat', 'female',  'Crème',           TRUE,  '2021-11-05', '2020-05-18',
     'Chatte persane douce et tranquille, adore les câlins.',
     NULL, 'shelter');

-- VACCINATIONS
INSERT INTO vaccinations (id_animal, vaccine_name, vaccine_date, is_done) VALUES
    -- Max
    ('26010100001', 'DHPPi/L',          '2024-03-15', TRUE),
    ('26010100001', 'Rage',             '2024-03-15', TRUE),
    ('26010100001', 'Rappel DHPPi/L',   '2025-03-20', FALSE),

    -- Luna
    ('26010100002', 'Typhus / Coryza',  '2023-07-22', TRUE),
    ('26010100002', 'Leucose féline',   '2023-07-22', TRUE),

    -- Rex
    ('26010100003', 'DHPPi/L',          '2024-11-03', TRUE),
    ('26010100003', 'Rage',             '2024-11-03', TRUE),

    -- Mia
    ('26010100004', 'Typhus / Coryza',  '2024-04-10', TRUE),

    -- Buddy
    ('26010100005', 'DHPPi/L',          '2024-06-30', TRUE),
    ('26010100005', 'Rage',             '2024-06-30', TRUE),

    -- Oscar
    ('26010100007', 'DHPPi/L',          '2024-09-12', TRUE),
    ('26010100007', 'Rappel DHPPi/L',   '2025-09-15', FALSE),

    -- Bella
    ('26010100008', 'Typhus / Coryza',  '2023-05-18', TRUE),
    ('26010100008', 'Leucose féline',   '2024-05-20', TRUE);

-- COMPATIBILITÉS
INSERT INTO compatibilities (id_animal, target_type, value, description) VALUES
    -- Max : bon avec chats et enfants, pas testé avec autres chiens
    ('26010100001', 'cat',         'yes',        'Vit avec un chat depuis toujours.'),
    ('26010100001', 'dog',         'not_tested',  NULL),
    ('26010100001', 'child',       'yes',        'Très doux avec les enfants de plus de 6 ans.'),
    ('26010100001', 'young_child', 'no',         'Peut bousculer les petits involontairement.'),
    ('26010100001', 'garden',      'yes',        NULL),

    -- Luna : préfère être seule, pas testée avec enfants
    ('26010100002', 'cat',         'no',         'Très territoriale, à adopter sans autre chat.'),
    ('26010100002', 'dog',         'no',         'Stressée en présence de chiens.'),
    ('26010100002', 'young_child', 'not_tested',  NULL),
    ('26010100002', 'child',       'yes',        'S''entend bien avec les enfants calmes.'),

    -- Rex : idéal en famille
    ('26010100003', 'cat',         'yes',        'Vit actuellement avec des chats sans problème.'),
    ('26010100003', 'dog',         'yes',        'Joueur, s''entend avec les autres chiens.'),
    ('26010100003', 'child',       'yes',        'Adore les enfants.'),
    ('26010100003', 'young_child', 'yes',        'Doux malgré son énergie.'),
    ('26010100003', 'garden',      'yes',        'Indispensable vu son niveau d''énergie.'),

    -- Oscar
    ('26010100007', 'cat',         'not_tested',  NULL),
    ('26010100007', 'dog',         'yes',        'Sociable avec ses congénères.'),
    ('26010100007', 'child',       'yes',        'Adoré par les enfants.'),
    ('26010100007', 'garden',      'yes',        'Indispensable, fugue si clôture insuffisante.');

-- HISTORIQUE D'ENTRÉES
INSERT INTO intake_histories (id_animal, id_person, intake_date, reason) VALUES
    ('26010100001', NULL,                                       '2024-01-10 10:00:00+01', 'stray'),
    ('26010100002', NULL,                                       '2023-08-15 14:30:00+02', 'stray'),
    ('26010100003', 'c1000000-0000-0000-0000-000000000001',    '2024-11-01 09:00:00+01', 'abandonment'),
    ('26010100004', 'c1000000-0000-0000-0000-000000000002',    '2025-01-20 11:00:00+01', 'abandonment'),
    ('26010100005', NULL,                                       '2022-07-01 08:00:00+02', 'stray'),
    ('26010100006', NULL,                                       '2024-01-10 16:00:00+01', 'stray'),
    ('26010100007', 'c1000000-0000-0000-0000-000000000001',    '2024-10-05 10:00:00+02', 'return'),
    ('26010100008', NULL,                                       '2023-06-01 09:30:00+02', 'stray');
