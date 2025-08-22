--
-- PostgreSQL database dump
--

-- Dumped from database version 9.0.1
-- Dumped by pg_dump version 9.0.1
-- Started on 2011-02-28 22:04:37

SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = off;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET escape_string_warning = off;

SET search_path = public, pg_catalog;

SET default_with_oids = false;

--
-- TOC entry 1598 (class 1259 OID 29597)
-- Dependencies: 5
-- Name: animal; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE animal (
    id integer NOT NULL,
    description character varying(255),
    body_weight double precision,
    mother_id integer,
    father_id integer,
    serialnumber character varying(255),
    parentid integer
);


--
-- TOC entry 1597 (class 1259 OID 29595)
-- Dependencies: 1598 5
-- Name: animal_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE animal_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2013 (class 0 OID 0)
-- Dependencies: 1597
-- Name: animal_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE animal_id_seq OWNED BY animal.id;


--
-- TOC entry 2014 (class 0 OID 0)
-- Dependencies: 1597
-- Name: animal_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('animal_id_seq', 6, true);


--
-- TOC entry 1587 (class 1259 OID 29548)
-- Dependencies: 5
-- Name: anotherentity; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE anotherentity (
    id integer NOT NULL,
    output character varying(255),
    input character varying(255),
    compositeobjectid integer,
    compositetenantid integer
);


--
-- TOC entry 1586 (class 1259 OID 29546)
-- Dependencies: 5 1587
-- Name: anotherentity_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE anotherentity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2015 (class 0 OID 0)
-- Dependencies: 1586
-- Name: anotherentity_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE anotherentity_id_seq OWNED BY anotherentity.id;


--
-- TOC entry 2016 (class 0 OID 0)
-- Dependencies: 1586
-- Name: anotherentity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('anotherentity_id_seq', 5, true);

CREATE TABLE compositeidentity (
    objectid integer NOT NULL,
    tenantid integer NOT NULL,
    name character varying(255)
);

--
-- TOC entry 1603 (class 1259 OID 29626)
-- Dependencies: 5
-- Name: cat; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE cat (
    mammal integer NOT NULL
);


--
-- TOC entry 1579 (class 1259 OID 29512)
-- Dependencies: 5
-- Name: categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE categories (
    categoryid integer NOT NULL,
    categoryname character varying(15) NOT NULL,
    description character varying(255)
);


--
-- TOC entry 1572 (class 1259 OID 29473)
-- Dependencies: 5
-- Name: customers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE customers (
    customerid character varying(255) NOT NULL,
    companyname character varying(40) NOT NULL,
    contactname character varying(30),
    contacttitle character varying(30),
    address character varying(60),
    city character varying(15),
    region character varying(15),
    postalcode character varying(10),
    country character varying(15),
    phone character varying(24),
    fax character varying(24)
);


--
-- TOC entry 1602 (class 1259 OID 29621)
-- Dependencies: 5
-- Name: dog; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE dog (
    mammal integer NOT NULL
);


--
-- TOC entry 1573 (class 1259 OID 29481)
-- Dependencies: 5
-- Name: employees; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE employees (
    employeeid integer NOT NULL,
    lastname character varying(20) NOT NULL,
    firstname character varying(10) NOT NULL,
    title character varying(30),
    titleofcourtesy character varying(25),
    birthdate timestamp without time zone,
    hiredate timestamp without time zone,
    address character varying(60),
    city character varying(15),
    region character varying(15),
    postalcode character varying(10),
    country character varying(15),
    homephone character varying(24),
    extension character varying(4),
    notes character varying(4000),
    reportsto integer
);


--
-- TOC entry 1574 (class 1259 OID 29489)
-- Dependencies: 5
-- Name: employeeterritories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE employeeterritories (
    employeeid integer NOT NULL,
    territoryid bigint NOT NULL
);


--
-- TOC entry 1600 (class 1259 OID 29611)
-- Dependencies: 5
-- Name: lizard; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE lizard (
    reptile integer NOT NULL
);


--
-- TOC entry 1601 (class 1259 OID 29616)
-- Dependencies: 5
-- Name: mammal; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE mammal (
    animal integer NOT NULL,
    pregnant boolean,
    birthdate timestamp without time zone
);


--
-- TOC entry 1577 (class 1259 OID 29501)
-- Dependencies: 5
-- Name: orderlines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE orderlines (
    orderlineid bigint NOT NULL,
    orderid integer NOT NULL,
    productid integer NOT NULL,
    unitprice numeric(19,5) NOT NULL,
    quantity integer NOT NULL,
    discount numeric(19,5) NOT NULL
);


--
-- TOC entry 1576 (class 1259 OID 29499)
-- Dependencies: 1577 5
-- Name: orderlines_orderlineid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE orderlines_orderlineid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2017 (class 0 OID 0)
-- Dependencies: 1576
-- Name: orderlines_orderlineid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE orderlines_orderlineid_seq OWNED BY orderlines.orderlineid;


--
-- TOC entry 2018 (class 0 OID 0)
-- Dependencies: 1576
-- Name: orderlines_orderlineid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('orderlines_orderlineid_seq', 2155, true);


--
-- TOC entry 1575 (class 1259 OID 29494)
-- Dependencies: 5
-- Name: orders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE orders (
    orderid integer NOT NULL,
    customerid character varying(255) NOT NULL,
    employeeid integer,
    orderdate timestamp without time zone,
    requireddate timestamp without time zone,
    shippeddate timestamp without time zone,
    shipvia integer,
    freight numeric(19,5),
    shipname character varying(40),
    shipaddress character varying(60),
    shipcity character varying(15),
    shipregion character varying(15),
    shippostalcode character varying(10),
    shipcountry character varying(15)
);


--
-- TOC entry 1609 (class 1259 OID 29649)
-- Dependencies: 5
-- Name: patientrecords; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE patientrecords (
    patientrecordid bigint NOT NULL,
    "Gender" integer NOT NULL,
    "BirthDate" timestamp without time zone NOT NULL,
    "FirstName" character varying(255) NOT NULL,
    "LastName" character varying(255) NOT NULL,
    "AddressLine1" character varying(255),
    "AddressLine2" character varying(255),
    "City" character varying(255),
    stateid bigint,
    "ZipCode" character varying(255),
    patientid bigint NOT NULL
);


--
-- TOC entry 1608 (class 1259 OID 29647)
-- Dependencies: 1609 5
-- Name: patientrecords_patientrecordid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE patientrecords_patientrecordid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2019 (class 0 OID 0)
-- Dependencies: 1608
-- Name: patientrecords_patientrecordid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE patientrecords_patientrecordid_seq OWNED BY patientrecords.patientrecordid;


--
-- TOC entry 2020 (class 0 OID 0)
-- Dependencies: 1608
-- Name: patientrecords_patientrecordid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('patientrecords_patientrecordid_seq', 3, true);


--
-- TOC entry 1605 (class 1259 OID 29633)
-- Dependencies: 5
-- Name: patients; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE patients (
    patientid bigint NOT NULL,
    "Active" boolean NOT NULL,
    physicianid bigint NOT NULL
);


--
-- TOC entry 1604 (class 1259 OID 29631)
-- Dependencies: 5 1605
-- Name: patients_patientid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE patients_patientid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2021 (class 0 OID 0)
-- Dependencies: 1604
-- Name: patients_patientid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE patients_patientid_seq OWNED BY patients.patientid;


--
-- TOC entry 2022 (class 0 OID 0)
-- Dependencies: 1604
-- Name: patients_patientid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('patients_patientid_seq', 2, true);


--
-- TOC entry 1607 (class 1259 OID 29641)
-- Dependencies: 5
-- Name: physicians; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE physicians (
    physicianid bigint NOT NULL,
    "Name" character varying(255) NOT NULL
);


--
-- TOC entry 1606 (class 1259 OID 29639)
-- Dependencies: 5 1607
-- Name: physicians_physicianid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE physicians_physicianid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2023 (class 0 OID 0)
-- Dependencies: 1606
-- Name: physicians_physicianid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE physicians_physicianid_seq OWNED BY physicians.physicianid;


--
-- TOC entry 2024 (class 0 OID 0)
-- Dependencies: 1606
-- Name: physicians_physicianid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('physicians_physicianid_seq', 2, true);


--
-- TOC entry 1578 (class 1259 OID 29507)
-- Dependencies: 5
-- Name: products; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE products (
    productid integer NOT NULL,
    productname character varying(40) NOT NULL,
    supplierid integer,
    categoryid integer,
    quantityperunit character varying(20),
    unitprice numeric(19,5),
    unitsinstock integer,
    unitsonorder integer,
    reorderlevel integer,
    discontinued boolean NOT NULL,
    shippingweight float
);


--
-- TOC entry 1581 (class 1259 OID 29519)
-- Dependencies: 5
-- Name: region; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE region (
    regionid bigint NOT NULL,
    regiondescription character varying(50) NOT NULL
);


--
-- TOC entry 1580 (class 1259 OID 29517)
-- Dependencies: 1581 5
-- Name: region_regionid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE region_regionid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2025 (class 0 OID 0)
-- Dependencies: 1580
-- Name: region_regionid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE region_regionid_seq OWNED BY region.regionid;


--
-- TOC entry 2026 (class 0 OID 0)
-- Dependencies: 1580
-- Name: region_regionid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('region_regionid_seq', 1, false);


--
-- TOC entry 1599 (class 1259 OID 29606)
-- Dependencies: 5
-- Name: reptile; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE reptile (
    animal integer NOT NULL,
    bodytemperature double precision
);


--
-- TOC entry 1589 (class 1259 OID 29559)
-- Dependencies: 5
-- Name: roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE roles (
    id integer NOT NULL,
    name character varying(255),
    isactive boolean,
    entityid integer,
    parentid integer
);


--
-- TOC entry 1588 (class 1259 OID 29557)
-- Dependencies: 1589 5
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2027 (class 0 OID 0)
-- Dependencies: 1588
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE roles_id_seq OWNED BY roles.id;


--
-- TOC entry 2028 (class 0 OID 0)
-- Dependencies: 1588
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('roles_id_seq', 2, true);


--
-- TOC entry 1582 (class 1259 OID 29525)
-- Dependencies: 5
-- Name: shippers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE shippers (
    shipperid integer NOT NULL,
    companyname character varying(40) NOT NULL,
    phone character varying(24),
    "Reference" uuid
);


--
-- TOC entry 1611 (class 1259 OID 29660)
-- Dependencies: 5
-- Name: states; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE states (
    stateid bigint NOT NULL,
    "Abbreviation" character varying(255) NOT NULL,
    "FullName" character varying(255) NOT NULL
);


--
-- TOC entry 1610 (class 1259 OID 29658)
-- Dependencies: 1611 5
-- Name: states_stateid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE states_stateid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2029 (class 0 OID 0)
-- Dependencies: 1610
-- Name: states_stateid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE states_stateid_seq OWNED BY states.stateid;


--
-- TOC entry 2030 (class 0 OID 0)
-- Dependencies: 1610
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('states_stateid_seq', 2, true);


--
-- TOC entry 1583 (class 1259 OID 29530)
-- Dependencies: 5
-- Name: suppliers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE suppliers (
    supplierid integer NOT NULL,
    companyname character varying(40) NOT NULL,
    contactname character varying(30),
    contacttitle character varying(30),
    homepage character varying(255),
    address character varying(60),
    city character varying(15),
    region character varying(15),
    postalcode character varying(10),
    country character varying(15),
    phone character varying(24),
    fax character varying(24)
);


--
-- TOC entry 1585 (class 1259 OID 29540)
-- Dependencies: 5
-- Name: territories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE territories (
    territoryid bigint NOT NULL,
    territorydescription character varying(50) NOT NULL,
    regionid bigint NOT NULL
);


--
-- TOC entry 1584 (class 1259 OID 29538)
-- Dependencies: 5 1585
-- Name: territories_territoryid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE territories_territoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2031 (class 0 OID 0)
-- Dependencies: 1584
-- Name: territories_territoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE territories_territoryid_seq OWNED BY territories.territoryid;


--
-- TOC entry 2032 (class 0 OID 0)
-- Dependencies: 1584
-- Name: territories_territoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('territories_territoryid_seq', 1, false);


--
-- TOC entry 1596 (class 1259 OID 29589)
-- Dependencies: 5
-- Name: timesheetentries; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE timesheetentries (
    timesheetentryid integer NOT NULL,
    entrydate timestamp without time zone,
    numberofhours integer,
    comments character varying(255),
    timesheetid integer
);


--
-- TOC entry 1595 (class 1259 OID 29587)
-- Dependencies: 5 1596
-- Name: timesheetentries_timesheetentryid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE timesheetentries_timesheetentryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2033 (class 0 OID 0)
-- Dependencies: 1595
-- Name: timesheetentries_timesheetentryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE timesheetentries_timesheetentryid_seq OWNED BY timesheetentries.timesheetentryid;


--
-- TOC entry 2034 (class 0 OID 0)
-- Dependencies: 1595
-- Name: timesheetentries_timesheetentryid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('timesheetentries_timesheetentryid_seq', 6, true);


--
-- TOC entry 1593 (class 1259 OID 29578)
-- Dependencies: 5
-- Name: timesheets; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE timesheets (
    timesheetid integer NOT NULL,
    submitteddate timestamp without time zone,
    submitted boolean
);


--
-- TOC entry 1592 (class 1259 OID 29576)
-- Dependencies: 1593 5
-- Name: timesheets_timesheetid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE timesheets_timesheetid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2035 (class 0 OID 0)
-- Dependencies: 1592
-- Name: timesheets_timesheetid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE timesheets_timesheetid_seq OWNED BY timesheets.timesheetid;


--
-- TOC entry 2036 (class 0 OID 0)
-- Dependencies: 1592
-- Name: timesheets_timesheetid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('timesheets_timesheetid_seq', 3, true);


--
-- TOC entry 1594 (class 1259 OID 29584)
-- Dependencies: 5
-- Name: timesheetusers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE timesheetusers (
    timesheetid integer NOT NULL,
    userid integer NOT NULL
);


--
-- TOC entry 1591 (class 1259 OID 29567)
-- Dependencies: 5
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE users (
    userid integer NOT NULL,
    name character varying(255),
    invalidloginattempts integer,
    registeredat timestamp without time zone,
    lastlogindate timestamp without time zone,
    enum1 character varying(12),
    enum2 integer NOT NULL,
    features integer NOT NULL,
    roleid integer,
    property1 character varying(255),
    property2 character varying(255),
    otherproperty1 character varying(255),
    createdbyid integer NOT NULL,
    modifiedbyid integer NULL
);

CREATE TABLE numericentity (
    short smallint not null constraint numericentity_pkey primary key,
    nullableshort smallint,
    "Integer" integer not null,
    nullableinteger integer,
    "Long" bigint not null,
    nullablelong bigint,
    "Decimal" numeric(19,5) not null,
    nullabledecimal numeric(19,5),
    "Single" real not null,
    nullablesingle real,
    "Double" double precision not null,
    nullabledouble double precision
);

--
-- TOC entry 1590 (class 1259 OID 29565)
-- Dependencies: 1591 5
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE users_userid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 2037 (class 0 OID 0)
-- Dependencies: 1590
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE users_userid_seq OWNED BY users.userid;


--
-- TOC entry 2038 (class 0 OID 0)
-- Dependencies: 1590
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('users_userid_seq', 3, true);


--
-- TOC entry 1897 (class 2604 OID 29600)
-- Dependencies: 1597 1598 1598
-- Name: id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE animal ALTER COLUMN id SET DEFAULT nextval('animal_id_seq'::regclass);


--
-- TOC entry 1892 (class 2604 OID 29551)
-- Dependencies: 1587 1586 1587
-- Name: id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE anotherentity ALTER COLUMN id SET DEFAULT nextval('anotherentity_id_seq'::regclass);


--
-- TOC entry 1889 (class 2604 OID 29504)
-- Dependencies: 1576 1577 1577
-- Name: orderlineid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE orderlines ALTER COLUMN orderlineid SET DEFAULT nextval('orderlines_orderlineid_seq'::regclass);


--
-- TOC entry 1900 (class 2604 OID 29652)
-- Dependencies: 1609 1608 1609
-- Name: patientrecordid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE patientrecords ALTER COLUMN patientrecordid SET DEFAULT nextval('patientrecords_patientrecordid_seq'::regclass);


--
-- TOC entry 1898 (class 2604 OID 29636)
-- Dependencies: 1605 1604 1605
-- Name: patientid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE patients ALTER COLUMN patientid SET DEFAULT nextval('patients_patientid_seq'::regclass);


--
-- TOC entry 1899 (class 2604 OID 29644)
-- Dependencies: 1607 1606 1607
-- Name: physicianid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE physicians ALTER COLUMN physicianid SET DEFAULT nextval('physicians_physicianid_seq'::regclass);


--
-- TOC entry 1890 (class 2604 OID 29522)
-- Dependencies: 1580 1581 1581
-- Name: regionid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE region ALTER COLUMN regionid SET DEFAULT nextval('region_regionid_seq'::regclass);


--
-- TOC entry 1893 (class 2604 OID 29562)
-- Dependencies: 1588 1589 1589
-- Name: id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE roles ALTER COLUMN id SET DEFAULT nextval('roles_id_seq'::regclass);


--
-- TOC entry 1901 (class 2604 OID 29663)
-- Dependencies: 1610 1611 1611
-- Name: stateid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE states ALTER COLUMN stateid SET DEFAULT nextval('states_stateid_seq'::regclass);


--
-- TOC entry 1891 (class 2604 OID 29543)
-- Dependencies: 1584 1585 1585
-- Name: territoryid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE territories ALTER COLUMN territoryid SET DEFAULT nextval('territories_territoryid_seq'::regclass);


--
-- TOC entry 1896 (class 2604 OID 29592)
-- Dependencies: 1596 1595 1596
-- Name: timesheetentryid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE timesheetentries ALTER COLUMN timesheetentryid SET DEFAULT nextval('timesheetentries_timesheetentryid_seq'::regclass);


--
-- TOC entry 1895 (class 2604 OID 29581)
-- Dependencies: 1592 1593 1593
-- Name: timesheetid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE timesheets ALTER COLUMN timesheetid SET DEFAULT nextval('timesheets_timesheetid_seq'::regclass);


--
-- TOC entry 1894 (class 2604 OID 29570)
-- Dependencies: 1591 1590 1591
-- Name: userid; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE users ALTER COLUMN userid SET DEFAULT nextval('users_userid_seq'::regclass);


--
-- TOC entry 1999 (class 0 OID 29597)
-- Dependencies: 1598
-- Data for Name: animal; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (1, NULL, 100, NULL, NULL, '123', NULL);
INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (2, NULL, 40, NULL, NULL, '789', NULL);
INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (3, NULL, 30, NULL, NULL, '1234', NULL);
INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (4, NULL, 156, NULL, NULL, '5678', 1);
INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (5, NULL, 205, NULL, NULL, '9101', 1);
INSERT INTO animal (id, description, body_weight, mother_id, father_id, serialnumber, parentid) VALUES (6, NULL, 115, 5, 4, '1121', 2);


--
-- TOC entry 1993 (class 0 OID 29548)
-- Dependencies: 1587
-- Data for Name: anotherentity; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO anotherentity (id, output, input) VALUES (1, NULL, 'input');
INSERT INTO anotherentity (id, output, input) VALUES (2, 'i/o', 'i/o');
INSERT INTO anotherentity (id, output, input) VALUES (3, NULL, NULL);
INSERT INTO anotherentity (id, output, input, compositeobjectid, compositetenantid) VALUES (4, 'output', 'input', 1, 10);
INSERT INTO anotherentity (id, output, input) VALUES (5, 'output', NULL);

INSERT INTO compositeidentity (objectid, tenantid, name) VALUES (1, 10, 'Jack Stephan');
--
-- TOC entry 2004 (class 0 OID 29626)
-- Dependencies: 1603
-- Data for Name: cat; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO cat (mammal) VALUES (6);


--
-- TOC entry 1988 (class 0 OID 29512)
-- Dependencies: 1579
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO categories (categoryid, categoryname, description) VALUES (1, 'Beverages', 'Soft drinks, coffees, teas, beers, and ales');
INSERT INTO categories (categoryid, categoryname, description) VALUES (2, 'Condiments', 'Sweet and savory sauces, relishes, spreads, and seasonings');
INSERT INTO categories (categoryid, categoryname, description) VALUES (3, 'Confections', 'Desserts, candies, and sweet breads');
INSERT INTO categories (categoryid, categoryname, description) VALUES (4, 'Dairy Products', 'Cheeses');
INSERT INTO categories (categoryid, categoryname, description) VALUES (5, 'Grains/Cereals', 'Breads, crackers, pasta, and cereal');
INSERT INTO categories (categoryid, categoryname, description) VALUES (6, 'Meat/Poultry', 'Prepared meats');
INSERT INTO categories (categoryid, categoryname, description) VALUES (7, 'Produce', 'Dried fruit and bean curd');
INSERT INTO categories (categoryid, categoryname, description) VALUES (8, 'Seafood', 'Seaweed and fish');


--
-- TOC entry 1982 (class 0 OID 29473)
-- Dependencies: 1572
-- Data for Name: customers; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ALFKI', 'Alfreds Futterkiste', 'Maria Anders', 'Sales Representative', 'Obere Str. 57', 'Berlin', '', '12209', 'Germany', '030-0074321', '030-0076545');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ANATR', 'Ana Trujillo Emparedados y helados', 'Ana Trujillo', 'Owner', 'Avda. de la Constitución 2222', 'México D.F.', '', '05021', 'Mexico', '(5) 555-4729', '(5) 555-3745');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ANTON', 'Antonio Moreno Taquería', 'Antonio Moreno', 'Owner', 'Mataderos  2312', 'México D.F.', '', '05023', 'Mexico', '(5) 555-3932', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('AROUT', 'Around the Horn', 'Thomas Hardy', 'Sales Representative', '120 Hanover Sq.', 'London', '', 'WA1 1DP', 'UK', '(171) 555-7788', '(171) 555-6750');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BERGS', 'Berglunds snabbköp', 'Christina Berglund', 'Order Administrator', 'Berguvsvägen  8', 'Luleå', '', 'S-958 22', 'Sweden', '0921-12 34 65', '0921-12 34 67');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BLAUS', 'Blauer See Delikatessen', 'Hanna Moos', 'Sales Representative', 'Forsterstr. 57', 'Mannheim', '', '68306', 'Germany', '0621-08460', '0621-08924');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BLONP', 'Blondesddsl père et fils', 'Frédérique Citeaux', 'Marketing Manager', '24, place Kléber', 'Strasbourg', '', '67000', 'France', '88.60.15.31', '88.60.15.32');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BOLID', 'Bólido Comidas preparadas', 'Martín Sommer', 'Owner', 'C/ Araquil, 67', 'Madrid', '', '28023', 'Spain', '(91) 555 22 82', '(91) 555 91 99');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BONAP', 'Bon app''', 'Laurence Lebihan', 'Owner', '12, rue des Bouchers', 'Marseille', '', '13008', 'France', '91.24.45.40', '91.24.45.41');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BOTTM', 'Bottom-Dollar Markets', 'Elizabeth Lincoln', 'Accounting Manager', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada', '(604) 555-4729', '(604) 555-3745');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('BSBEV', 'B''s Beverages', 'Victoria Ashworth', 'Sales Representative', 'Fauntleroy Circus', 'London', '', 'EC2 5NT', 'UK', '(171) 555-1212', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('CACTU', 'Cactus Comidas para llevar', 'Patricio Simpson', 'Sales Agent', 'Cerrito 333', 'Buenos Aires', '', '1010', 'Argentina', '(1) 135-5555', '(1) 135-4892');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('CENTC', 'Centro comercial Moctezuma', 'Francisco Chang', 'Marketing Manager', 'Sierras de Granada 9993', 'México D.F.', '', '05022', 'Mexico', '(5) 555-3392', '(5) 555-7293');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('CHOPS', 'Chop-suey Chinese', 'Yang Wang', 'Owner', 'Hauptstr. 29', 'Bern', '', '3012', 'Switzerland', '0452-076545', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('COMMI', 'Comércio Mineiro', 'Pedro Afonso', 'Sales Associate', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil', '(11) 555-7647', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('CONSH', 'Consolidated Holdings', 'Elizabeth Brown', 'Sales Representative', 'Berkeley Gardens 12  Brewery', 'London', '', 'WX1 6LT', 'UK', '(171) 555-2282', '(171) 555-9199');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('DRACD', 'Drachenblut Delikatessen', 'Sven Ottlieb', 'Order Administrator', 'Walserweg 21', 'Aachen', '', '52066', 'Germany', '0241-039123', '0241-059428');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('DUMON', 'Du monde entier', 'Janine Labrune', 'Owner', '67, rue des Cinquante Otages', 'Nantes', '', '44000', 'France', '40.67.88.88', '40.67.89.89');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('EASTC', 'Eastern Connection', 'Ann Devon', 'Sales Agent', '35 King George', 'London', '', 'WX3 6FW', 'UK', '(171) 555-0297', '(171) 555-3373');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ERNSH', 'Ernst Handel', 'Roland Mendel', 'Sales Manager', 'Kirchgasse 6', 'Graz', '', '8010', 'Austria', '7675-3425', '7675-3426');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FAMIA', 'Familia Arquibaldo', 'Aria Cruz', 'Marketing Assistant', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil', '(11) 555-9857', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FISSA', 'FISSA Fabrica Inter. Salchichas S.A.', 'Diego Roel', 'Accounting Manager', 'C/ Moralzarzal, 86', 'Madrid', '', '28034', 'Spain', '(91) 555 94 44', '(91) 555 55 93');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FOLIG', 'Folies gourmandes', 'Martine Rancé', 'Assistant Sales Agent', '184, chaussée de Tournai', 'Lille', '', '59000', 'France', '20.16.10.16', '20.16.10.17');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FOLKO', 'Folk och fä HB', 'Maria Larsson', 'Owner', 'Åkergatan 24', 'Bräcke', '', 'S-844 67', 'Sweden', '0695-34 67 21', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FRANK', 'Frankenversand', 'Peter Franken', 'Marketing Manager', 'Berliner Platz 43', 'München', '', '80805', 'Germany', '089-0877310', '089-0877451');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FRANR', 'France restauration', 'Carine Schmitt', 'Marketing Manager', '54, rue Royale', 'Nantes', '', '44000', 'France', '40.32.21.21', '40.32.21.20');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FRANS', 'Franchi S.p.A.', 'Paolo Accorti', 'Sales Representative', 'Via Monte Bianco 34', 'Torino', '', '10100', 'Italy', '011-4988260', '011-4988261');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('FURIB', 'Furia Bacalhau e Frutos do Mar', 'Lino Rodriguez', 'Sales Manager', 'Jardim das rosas n. 32', 'Lisboa', '', '1675', 'Portugal', '(1) 354-2534', '(1) 354-2535');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('GALED', 'Galería del gastrónomo', 'Eduardo Saavedra', 'Marketing Manager', 'Rambla de Cataluña, 23', 'Barcelona', '', '08022', 'Spain', '(93) 203 4560', '(93) 203 4561');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('GODOS', 'Godos Cocina Típica', 'José Pedro Freyre', 'Sales Manager', 'C/ Romero, 33', 'Sevilla', '', '41101', 'Spain', '(95) 555 82 82', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('GOURL', 'Gourmet Lanchonetes', 'André Fonseca', 'Sales Associate', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil', '(11) 555-9482', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('GREAL', 'Great Lakes Food Market', 'Howard Snyder', 'Marketing Manager', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA', '(503) 555-7555', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('GROSR', 'GROSELLA-Restaurante', 'Manuel Pereira', 'Owner', '5ª Ave. Los Palos Grandes', 'Caracas', 'DF', '1081', 'Venezuela', '(2) 283-2951', '(2) 283-3397');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('HANAR', 'Hanari Carnes', 'Mario Pontes', 'Accounting Manager', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil', '(21) 555-0091', '(21) 555-8765');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('HILAA', 'HILARION-Abastos', 'Carlos Hernández', 'Sales Representative', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela', '(5) 555-1340', '(5) 555-1948');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('HUNGC', 'Hungry Coyote Import Store', 'Yoshi Latimer', 'Sales Representative', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA', '(503) 555-6874', '(503) 555-2376');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('HUNGO', 'Hungry Owl All-Night Grocers', 'Patricia McKenna', 'Sales Associate', '8 Johnstown Road', 'Cork', 'Co. Cork', '', 'Ireland', '2967 542', '2967 3333');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ISLAT', 'Island Trading', 'Helen Bennett', 'Marketing Manager', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK', '(198) 555-8888', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('KOENE', 'Königlich Essen', 'Philip Cramer', 'Sales Associate', 'Maubelstr. 90', 'Brandenburg', '', '14776', 'Germany', '0555-09876', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LACOR', 'La corne d''abondance', 'Daniel Tonini', 'Sales Representative', '67, avenue de l''Europe', 'Versailles', '', '78000', 'France', '30.59.84.10', '30.59.85.11');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LAMAI', 'La maison d''Asie', 'Annette Roulet', 'Sales Manager', '1 rue Alsace-Lorraine', 'Toulouse', '', '31000', 'France', '61.77.61.10', '61.77.61.11');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LAUGB', 'Laughing Bacchus Wine Cellars', 'Yoshi Tannamuri', 'Marketing Assistant', '1900 Oak St.', 'Vancouver', 'BC', 'V3F 2K1', 'Canada', '(604) 555-3392', '(604) 555-7293');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LAZYK', 'Lazy K Kountry Store', 'John Steel', 'Marketing Manager', '12 Orchestra Terrace', 'Walla Walla', 'WA', '99362', 'USA', '(509) 555-7969', '(509) 555-6221');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LEHMS', 'Lehmanns Marktstand', 'Renate Messner', 'Sales Representative', 'Magazinweg 7', 'Frankfurt a.M.', '', '60528', 'Germany', '069-0245984', '069-0245874');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LETSS', 'Let''s Stop N Shop', 'Jaime Yorres', 'Owner', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA', '(415) 555-5938', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LILAS', 'LILA-Supermercado', 'Carlos González', 'Accounting Manager', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela', '(9) 331-6954', '(9) 331-7256');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LINOD', 'LINO-Delicateses', 'Felipe Izquierdo', 'Owner', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela', '(8) 34-56-12', '(8) 34-93-93');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('LONEP', 'Lonesome Pine Restaurant', 'Fran Wilson', 'Sales Manager', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA', '(503) 555-9573', '(503) 555-9646');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('MAGAA', 'Magazzini Alimentari Riuniti', 'Giovanni Rovelli', 'Marketing Manager', 'Via Ludovico il Moro 22', 'Bergamo', '', '24100', 'Italy', '035-640230', '035-640231');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('MAISD', 'Maison Dewey', 'Catherine Dewey', 'Sales Agent', 'Rue Joseph-Bens 532', 'Bruxelles', '', 'B-1180', 'Belgium', '(02) 201 24 67', '(02) 201 24 68');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('MEREP', 'Mère Paillarde', 'Jean Fresnière', 'Marketing Assistant', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada', '(514) 555-8054', '(514) 555-8055');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('MORGK', 'Morgenstern Gesundkost', 'Alexander Feuer', 'Marketing Assistant', 'Heerstr. 22', 'Leipzig', '', '04179', 'Germany', '0342-023176', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('NORTS', 'North/South', 'Simon Crowther', 'Sales Associate', 'South House 300 Queensbridge', 'London', '', 'SW7 1RZ', 'UK', '(171) 555-7733', '(171) 555-2530');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('OCEAN', 'Océano Atlántico Ltda.', 'Yvonne Moncada', 'Sales Agent', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', '', '1010', 'Argentina', '(1) 135-5333', '(1) 135-5535');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('OLDWO', 'Old World Delicatessen', 'Rene Phillips', 'Sales Representative', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA', '(907) 555-7584', '(907) 555-2880');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('OTTIK', 'Ottilies Käseladen', 'Henriette Pfalzheim', 'Owner', 'Mehrheimerstr. 369', 'Köln', '', '50739', 'Germany', '0221-0644327', '0221-0765721');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('PARIS', 'Paris spécialités', 'Marie Bertrand', 'Owner', '265, boulevard Charonne', 'Paris', '', '75012', 'France', '(1) 42.34.22.66', '(1) 42.34.22.77');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('PERIC', 'Pericles Comidas clásicas', 'Guillermo Fernández', 'Sales Representative', 'Calle Dr. Jorge Cash 321', 'México D.F.', '', '05033', 'Mexico', '(5) 552-3745', '(5) 545-3745');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('PICCO', 'Piccolo und mehr', 'Georg Pipps', 'Sales Manager', 'Geislweg 14', 'Salzburg', '', '5020', 'Austria', '6562-9722', '6562-9723');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('PRINI', 'Princesa Isabel Vinhos', 'Isabel de Castro', 'Sales Representative', 'Estrada da saúde n. 58', 'Lisboa', '', '1756', 'Portugal', '(1) 356-5634', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('QUEDE', 'Que Delícia', 'Bernardo Batista', 'Accounting Manager', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil', '(21) 555-4252', '(21) 555-4545');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('QUEEN', 'Queen Cozinha', 'Lúcia Carvalho', 'Marketing Assistant', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil', '(11) 555-1189', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('QUICK', 'QUICK-Stop', 'Horst Kloss', 'Accounting Manager', 'Taucherstraße 10', 'Cunewalde', '', '01307', 'Germany', '0372-035188', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('RANCH', 'Rancho grande', 'Sergio Gutiérrez', 'Sales Representative', 'Av. del Libertador 900', 'Buenos Aires', '', '1010', 'Argentina', '(1) 123-5555', '(1) 123-5556');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('RATTC', 'Rattlesnake Canyon Grocery', 'Paula Wilson', 'Assistant Sales Representative', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA', '(505) 555-5939', '(505) 555-3620');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('REGGC', 'Reggiani Caseifici', 'Maurizio Moroni', 'Sales Associate', 'Strada Provinciale 124', 'Reggio Emilia', '', '42100', 'Italy', '0522-556721', '0522-556722');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('RICAR', 'Ricardo Adocicados', 'Janete Limeira', 'Assistant Sales Agent', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil', '(21) 555-3412', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('RICSU', 'Richter Supermarkt', 'Michael Holz', 'Sales Manager', 'Grenzacherweg 237', 'Genève', '', '1203', 'Switzerland', '0897-034214', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('ROMEY', 'Romero y tomillo', 'Alejandra Camino', 'Accounting Manager', 'Gran Vía, 1', 'Madrid', '', '28001', 'Spain', '(91) 745 6200', '(91) 745 6210');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SANTG', 'Santé Gourmet', 'Jonas Bergulfsen', 'Owner', 'Erling Skakkes gate 78', 'Stavern', '', '4110', 'Norway', '07-98 92 35', '07-98 92 47');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SAVEA', 'Save-a-lot Markets', 'Jose Pavarotti', 'Sales Representative', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA', '(208) 555-8097', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SEVES', 'Seven Seas Imports', 'Hari Kumar', 'Sales Manager', '90 Wadhurst Rd.', 'London', '', 'OX15 4NB', 'UK', '(171) 555-1717', '(171) 555-5646');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SIMOB', 'Simons bistro', 'Jytte Petersen', 'Owner', 'Vinbæltet 34', 'Kobenhavn', '', '1734', 'Denmark', '31 12 34 56', '31 13 35 57');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SPECD', 'Spécialités du monde', 'Dominique Perrier', 'Marketing Manager', '25, rue Lauriston', 'Paris', '', '75016', 'France', '(1) 47.55.60.10', '(1) 47.55.60.20');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SPLIR', 'Split Rail Beer & Ale', 'Art Braunschweiger', 'Sales Manager', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA', '(307) 555-4680', '(307) 555-6525');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('SUPRD', 'Suprêmes délices', 'Pascale Cartrain', 'Accounting Manager', 'Boulevard Tirou, 255', 'Charleroi', '', 'B-6000', 'Belgium', '(071) 23 67 22 20', '(071) 23 67 22 21');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('THEBI', 'The Big Cheese', 'Liz Nixon', 'Marketing Manager', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA', '(503) 555-3612', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('THECR', 'The Cracker Box', 'Liu Wong', 'Marketing Assistant', '55 Grizzly Peak Rd.', 'Butte', 'MT', '59801', 'USA', '(406) 555-5834', '(406) 555-8083');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('TOMSP', 'Toms Spezialitäten', 'Karin Josephs', 'Marketing Manager', 'Luisenstr. 48', 'Münster', '', '44087', 'Germany', '0251-031259', '0251-035695');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('TORTU', 'Tortuga Restaurante', 'Miguel Angel Paolino', 'Owner', 'Avda. Azteca 123', 'México D.F.', '', '05033', 'Mexico', '(5) 555-2933', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('TRADH', 'Tradição Hipermercados', 'Anabela Domingues', 'Sales Representative', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil', '(11) 555-2167', '(11) 555-2168');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('TRAIH', 'Trail''s Head Gourmet Provisioners', 'Helvetius Nagy', 'Sales Associate', '722 DaVinci Blvd.', 'Kirkland', 'WA', '98034', 'USA', '(206) 555-8257', '(206) 555-2174');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('VAFFE', 'Vaffeljernet', 'Palle Ibsen', 'Sales Manager', 'Smagsloget 45', 'Århus', '', '8200', 'Denmark', '86 21 32 43', '86 22 33 44');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('VICTE', 'Victuailles en stock', 'Mary Saveley', 'Sales Agent', '2, rue du Commerce', 'Lyon', '', '69004', 'France', '78.32.54.86', '78.32.54.87');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('VINET', 'Vins et alcools Chevalier', 'Paul Henriot', 'Accounting Manager', '59 rue de l''Abbaye', 'Reims', '', '51100', 'France', '26.47.15.10', '26.47.15.11');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WANDK', 'Die Wandernde Kuh', 'Rita Müller', 'Sales Representative', 'Adenauerallee 900', 'Stuttgart', '', '70563', 'Germany', '0711-020361', '0711-035428');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WARTH', 'Wartian Herkku', 'Pirkko Koskitalo', 'Accounting Manager', 'Torikatu 38', 'Oulu', '', '90110', 'Finland', '981-443655', '981-443655');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WELLI', 'Wellington Importadora', 'Paula Parente', 'Sales Manager', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil', '(14) 555-8122', '');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WHITC', 'White Clover Markets', 'Karl Jablonski', 'Owner', '305 - 14th Ave. S. Suite 3B', 'Seattle', 'WA', '98128', 'USA', '(206) 555-4112', '(206) 555-4115');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WILMK', 'Wilman Kala', 'Matti Karttunen', 'Owner/Marketing Assistant', 'Keskuskatu 45', 'Helsinki', '', '21240', 'Finland', '90-224 8858', '90-224 8858');
INSERT INTO customers (customerid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax) VALUES ('WOLZA', 'Wolski  Zajazd', 'Zbyszek Piestrzeniewicz', 'Owner', 'ul. Filtrowa 68', 'Warszawa', '', '01-012', 'Poland', '(26) 642-7012', '(26) 642-7012');


--
-- TOC entry 2003 (class 0 OID 29621)
-- Dependencies: 1602
-- Data for Name: dog; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO dog (mammal) VALUES (4);
INSERT INTO dog (mammal) VALUES (5);


--
-- TOC entry 1983 (class 0 OID 29481)
-- Dependencies: 1573
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (1, 'Davolio', 'Nancy', 'Sales Representative', 'Ms.', '1948-12-08 00:00:00', '1992-05-01 00:00:00', '507 - 20th Ave. E.Apt. 2A', 'Seattle', 'WA', '98122', 'USA', '(206) 555-9857', '5467', 'Education includes a BA in psychology from Colorado State University in 1970.  She also completed ''The Art of the Cold Call.''  Nancy is a member of Toastmasters International.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (2, 'Fuller', 'Andrew', 'Vice President, Sales', 'Dr.', '1952-02-19 00:00:00', '1992-08-14 00:00:00', '908 W. Capital Way', 'Tacoma', 'WA', '98401', 'USA', '(206) 555-9482', '3457', 'Andrew received his BTS commercial in 1974 and a Ph.D. in international marketing from the University of Dallas in 1981.  He is fluent in French and Italian and reads German.  He joined the company as a sales representative, was promoted to sales manager in January 1992 and to vice president of sales in March 1993.  Andrew is a member of the Sales Management Roundtable, the Seattle Chamber of Commerce, and the Pacific Rim Importers Association.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (3, 'Leverling', 'Janet', 'Sales Representative', 'Ms.', '1963-08-30 00:00:00', '1992-04-01 00:00:00', '722 Moss Bay Blvd.', 'Kirkland', 'WA', '98033', 'USA', '(206) 555-3412', '3355', 'Janet has a BS degree in chemistry from Boston College (1984).  She has also completed a certificate program in food retailing management.  Janet was hired as a sales associate in 1991 and promoted to sales representative in February 1992.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (4, 'Peacock', 'Margaret', 'Sales Representative', 'Mrs.', '1937-09-19 00:00:00', '1993-05-03 00:00:00', '4110 Old Redmond Rd.', 'Redmond', 'WA', '98052', 'USA', '(206) 555-8122', '5176', 'Margaret holds a BA in English literature from Concordia College (1958) and an MA from the American Institute of Culinary Arts (1966).  She was assigned to the London office temporarily from July through November 1992.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (5, 'Buchanan', 'Steven', 'Sales Manager', 'Mr.', '1955-03-04 00:00:00', '1993-10-17 00:00:00', '14 Garrett Hill', 'London', '', 'SW1 8JR', 'UK', '(71) 555-4848', '3453', 'Steven Buchanan graduated from St. Andrews University, Scotland, with a BSC degree in 1976.  Upon joining the company as a sales representative in 1992, he spent 6 months in an orientation program at the Seattle office and then returned to his permanent post in London.  He was promoted to sales manager in March 1993.  Mr. Buchanan has completed the courses ''Successful Telemarketing'' and ''International Sales Management.''  He is fluent in French.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (6, 'Suyama', 'Michael', 'Sales Representative', 'Mr.', '1963-07-02 00:00:00', '1993-10-17 00:00:00', 'Coventry HouseMiner Rd.', 'London', '', 'EC2 7JR', 'UK', '(71) 555-7773', '428', 'Michael is a graduate of Sussex University (MA, economics, 1983) and the University of California at Los Angeles (MBA, marketing, 1986).  He has also taken the courses ''Multi-Cultural Selling'' and ''Time Management for the Sales Professional.''  He is fluent in Japanese and can read and write French, Portuguese, and Spanish.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (7, 'King', 'Robert', 'Sales Representative', 'Mr.', '1960-05-29 00:00:00', '1994-01-02 00:00:00', 'Edgeham HollowWinchester Way', 'London', '', 'RG1 9SP', 'UK', '(71) 555-5598', '465', 'Robert King served in the Peace Corps and traveled extensively before completing his degree in English at the University of Michigan in 1992, the year he joined the company.  After completing a course entitled ''Selling in Europe,'' he was transferred to the London office in March 1993.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (8, 'Callahan', 'Laura', 'Inside Sales Coordinator', 'Ms.', '1958-01-09 00:00:00', '1994-03-05 00:00:00', '4726 - 11th Ave. N.E.', 'Seattle', 'WA', '98105', 'USA', '(206) 555-1189', '2344', 'Laura received a BA in psychology from the University of Washington.  She has also completed a course in business French.  She reads and writes French.', NULL);
INSERT INTO employees (employeeid, lastname, firstname, title, titleofcourtesy, birthdate, hiredate, address, city, region, postalcode, country, homephone, extension, notes, reportsto) VALUES (9, 'Dodsworth', 'Anne', 'Sales Representative', 'Ms.', '1966-01-27 00:00:00', '1994-11-15 00:00:00', '7 Houndstooth Rd.', 'London', '', 'WG2 7LT', 'UK', '(71) 555-4444', '452', 'Anne has a BA degree in English from St. Lawrence College.  She is fluent in French and German.', NULL);


--
-- TOC entry 1984 (class 0 OID 29489)
-- Dependencies: 1574
-- Data for Name: employeeterritories; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2001 (class 0 OID 29611)
-- Dependencies: 1600
-- Data for Name: lizard; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO lizard (reptile) VALUES (2);
INSERT INTO lizard (reptile) VALUES (3);


--
-- TOC entry 2002 (class 0 OID 29616)
-- Dependencies: 1601
-- Data for Name: mammal; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO mammal (animal, pregnant, birthdate) VALUES (4, false, '1980-07-11 00:00:00');
INSERT INTO mammal (animal, pregnant, birthdate) VALUES (5, false, '1980-12-13 00:00:00');
INSERT INTO mammal (animal, pregnant, birthdate) VALUES (6, true, NULL);


--
-- TOC entry 1986 (class 0 OID 29501)
-- Dependencies: 1577
-- Data for Name: orderlines; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1, 10248, 11, 14.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2, 10248, 42, 9.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (3, 10248, 72, 34.80000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (4, 10249, 14, 18.60000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (5, 10249, 51, 42.40000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (6, 10250, 41, 7.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (7, 10250, 51, 42.40000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (8, 10250, 65, 16.80000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (9, 10251, 22, 16.80000, 6, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (10, 10251, 57, 15.60000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (11, 10251, 65, 16.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (12, 10252, 20, 64.80000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (13, 10252, 33, 2.00000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (14, 10252, 60, 27.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (15, 10253, 31, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (16, 10253, 39, 14.40000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (17, 10253, 49, 16.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (18, 10254, 24, 3.60000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (19, 10254, 55, 19.20000, 21, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (20, 10254, 74, 8.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (21, 10255, 2, 15.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (22, 10255, 16, 13.90000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (23, 10255, 36, 15.20000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (24, 10255, 59, 44.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (25, 10256, 53, 26.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (26, 10256, 77, 10.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (27, 10257, 27, 35.10000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (28, 10257, 39, 14.40000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (29, 10257, 77, 10.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (30, 10258, 2, 15.20000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (31, 10258, 5, 17.00000, 65, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (32, 10258, 32, 25.60000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (33, 10259, 21, 8.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (34, 10259, 37, 20.80000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (35, 10260, 41, 7.70000, 16, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (36, 10260, 57, 15.60000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (37, 10260, 62, 39.40000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (38, 10260, 70, 12.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (39, 10261, 21, 8.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (40, 10261, 35, 14.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (41, 10262, 5, 17.00000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (42, 10262, 7, 24.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (43, 10262, 56, 30.40000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (44, 10263, 16, 13.90000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (45, 10263, 24, 3.60000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (46, 10263, 30, 20.70000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (47, 10263, 74, 8.00000, 36, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (48, 10264, 2, 15.20000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (49, 10264, 41, 7.70000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (50, 10265, 17, 31.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (51, 10265, 70, 12.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (52, 10266, 12, 30.40000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (53, 10267, 40, 14.70000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (54, 10267, 59, 44.00000, 70, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (55, 10267, 76, 14.40000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (56, 10268, 29, 99.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (57, 10268, 72, 27.80000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (58, 10269, 33, 2.00000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (59, 10269, 72, 27.80000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (60, 10270, 36, 15.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (61, 10270, 43, 36.80000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (62, 10271, 33, 2.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (63, 10272, 20, 64.80000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (64, 10272, 31, 10.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (65, 10272, 72, 27.80000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (66, 10273, 10, 24.80000, 24, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (67, 10273, 31, 10.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (68, 10273, 33, 2.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (69, 10273, 40, 14.70000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (70, 10273, 76, 14.40000, 33, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (71, 10274, 71, 17.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (72, 10274, 72, 27.80000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (73, 10275, 24, 3.60000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (74, 10275, 59, 44.00000, 6, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (75, 10276, 10, 24.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (76, 10276, 13, 4.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (77, 10277, 28, 36.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (78, 10277, 62, 39.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (79, 10278, 44, 15.50000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (80, 10278, 59, 44.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (81, 10278, 63, 35.10000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (82, 10278, 73, 12.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (83, 10279, 17, 31.20000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (84, 10280, 24, 3.60000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (85, 10280, 55, 19.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (86, 10280, 75, 6.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (87, 10281, 19, 7.30000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (88, 10281, 24, 3.60000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (89, 10281, 35, 14.40000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (90, 10282, 30, 20.70000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (91, 10282, 57, 15.60000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (92, 10283, 15, 12.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (93, 10283, 19, 7.30000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (94, 10283, 60, 27.20000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (95, 10283, 72, 27.80000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (96, 10284, 27, 35.10000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (97, 10284, 44, 15.50000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (98, 10284, 60, 27.20000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (99, 10284, 67, 11.20000, 5, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (100, 10285, 1, 14.40000, 45, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (101, 10285, 40, 14.70000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (102, 10285, 53, 26.20000, 36, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (103, 10286, 35, 14.40000, 100, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (104, 10286, 62, 39.40000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (105, 10287, 16, 13.90000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (106, 10287, 34, 11.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (107, 10287, 46, 9.60000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (108, 10288, 54, 5.90000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (109, 10288, 68, 10.00000, 3, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (110, 10289, 3, 8.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (111, 10289, 64, 26.60000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (112, 10290, 5, 17.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (113, 10290, 29, 99.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (114, 10290, 49, 16.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (115, 10290, 77, 10.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (116, 10291, 13, 4.80000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (117, 10291, 44, 15.50000, 24, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (118, 10291, 51, 42.40000, 2, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (119, 10292, 20, 64.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (120, 10293, 18, 50.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (121, 10293, 24, 3.60000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (122, 10293, 63, 35.10000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (123, 10293, 75, 6.20000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (124, 10294, 1, 14.40000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (125, 10294, 17, 31.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (126, 10294, 43, 36.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (127, 10294, 60, 27.20000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (128, 10294, 75, 6.20000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (129, 10295, 56, 30.40000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (130, 10296, 11, 16.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (131, 10296, 16, 13.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (132, 10296, 69, 28.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (133, 10297, 39, 14.40000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (134, 10297, 72, 27.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (135, 10298, 2, 15.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (136, 10298, 36, 15.20000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (137, 10298, 59, 44.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (138, 10298, 62, 39.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (139, 10299, 19, 7.30000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (140, 10299, 70, 12.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (141, 10300, 66, 13.60000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (142, 10300, 68, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (143, 10301, 40, 14.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (144, 10301, 56, 30.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (145, 10302, 17, 31.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (146, 10302, 28, 36.40000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (147, 10302, 43, 36.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (148, 10303, 40, 14.70000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (149, 10303, 65, 16.80000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (150, 10303, 68, 10.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (151, 10304, 49, 16.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (152, 10304, 59, 44.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (153, 10304, 71, 17.20000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (154, 10305, 18, 50.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (155, 10305, 29, 99.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (156, 10305, 39, 14.40000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (157, 10306, 30, 20.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (158, 10306, 53, 26.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (159, 10306, 54, 5.90000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (160, 10307, 62, 39.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (161, 10307, 68, 10.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (162, 10308, 69, 28.80000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (163, 10308, 70, 12.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (164, 10309, 4, 17.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (165, 10309, 6, 20.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (166, 10309, 42, 11.20000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (167, 10309, 43, 36.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (168, 10309, 71, 17.20000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (169, 10310, 16, 13.90000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (170, 10310, 62, 39.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (171, 10311, 42, 11.20000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (172, 10311, 69, 28.80000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (173, 10312, 28, 36.40000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (174, 10312, 43, 36.80000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (175, 10312, 53, 26.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (176, 10312, 75, 6.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (177, 10313, 36, 15.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (178, 10314, 32, 25.60000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (179, 10314, 58, 10.60000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (180, 10314, 62, 39.40000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (181, 10315, 34, 11.20000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (182, 10315, 70, 12.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (183, 10316, 41, 7.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (184, 10316, 62, 39.40000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (185, 10317, 1, 14.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (186, 10318, 41, 7.70000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (187, 10318, 76, 14.40000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (188, 10319, 17, 31.20000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (189, 10319, 28, 36.40000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (190, 10319, 76, 14.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (191, 10320, 71, 17.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (192, 10321, 35, 14.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (193, 10322, 52, 5.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (194, 10323, 15, 12.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (195, 10323, 25, 11.20000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (196, 10323, 39, 14.40000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (197, 10324, 16, 13.90000, 21, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (198, 10324, 35, 14.40000, 70, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (199, 10324, 46, 9.60000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (200, 10324, 59, 44.00000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (201, 10324, 63, 35.10000, 80, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (202, 10325, 6, 20.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (203, 10325, 13, 4.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (204, 10325, 14, 18.60000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (205, 10325, 31, 10.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (206, 10325, 72, 27.80000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (207, 10326, 4, 17.60000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (208, 10326, 57, 15.60000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (209, 10326, 75, 6.20000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (210, 10327, 2, 15.20000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (211, 10327, 11, 16.80000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (212, 10327, 30, 20.70000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (213, 10327, 58, 10.60000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (214, 10328, 59, 44.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (215, 10328, 65, 16.80000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (216, 10328, 68, 10.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (217, 10329, 19, 7.30000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (218, 10329, 30, 20.70000, 8, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (219, 10329, 38, 210.80000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (220, 10329, 56, 30.40000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (221, 10330, 26, 24.90000, 50, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (222, 10330, 72, 27.80000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (223, 10331, 54, 5.90000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (224, 10332, 18, 50.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (225, 10332, 42, 11.20000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (226, 10332, 47, 7.60000, 16, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (227, 10333, 14, 18.60000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (228, 10333, 21, 8.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (229, 10333, 71, 17.20000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (230, 10334, 52, 5.60000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (231, 10334, 68, 10.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (232, 10335, 2, 15.20000, 7, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (233, 10335, 31, 10.00000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (234, 10335, 32, 25.60000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (235, 10335, 51, 42.40000, 48, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (236, 10336, 4, 17.60000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (237, 10337, 23, 7.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (238, 10337, 26, 24.90000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (239, 10337, 36, 15.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (240, 10337, 37, 20.80000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (241, 10337, 72, 27.80000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (242, 10338, 17, 31.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (243, 10338, 30, 20.70000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (244, 10339, 4, 17.60000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (245, 10339, 17, 31.20000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (246, 10339, 62, 39.40000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (247, 10340, 18, 50.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (248, 10340, 41, 7.70000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (249, 10340, 43, 36.80000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (250, 10341, 33, 2.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (251, 10341, 59, 44.00000, 9, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (252, 10342, 2, 15.20000, 24, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (253, 10342, 31, 10.00000, 56, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (254, 10342, 36, 15.20000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (255, 10342, 55, 19.20000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (256, 10343, 64, 26.60000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (257, 10343, 68, 10.00000, 4, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (258, 10343, 76, 14.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (259, 10344, 4, 17.60000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (260, 10344, 8, 32.00000, 70, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (261, 10345, 8, 32.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (262, 10345, 19, 7.30000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (263, 10345, 42, 11.20000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (264, 10346, 17, 31.20000, 36, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (265, 10346, 56, 30.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (266, 10347, 25, 11.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (267, 10347, 39, 14.40000, 50, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (268, 10347, 40, 14.70000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (269, 10347, 75, 6.20000, 6, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (270, 10348, 1, 14.40000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (271, 10348, 23, 7.20000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (272, 10349, 54, 5.90000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (273, 10350, 50, 13.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (274, 10350, 69, 28.80000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (275, 10351, 38, 210.80000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (276, 10351, 41, 7.70000, 13, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (277, 10351, 44, 15.50000, 77, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (278, 10351, 65, 16.80000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (279, 10352, 24, 3.60000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (280, 10352, 54, 5.90000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (281, 10353, 11, 16.80000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (282, 10353, 38, 210.80000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (283, 10354, 1, 14.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (284, 10354, 29, 99.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (285, 10355, 24, 3.60000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (286, 10355, 57, 15.60000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (287, 10356, 31, 10.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (288, 10356, 55, 19.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (289, 10356, 69, 28.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (290, 10357, 10, 24.80000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (291, 10357, 26, 24.90000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (292, 10357, 60, 27.20000, 8, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (293, 10358, 24, 3.60000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (294, 10358, 34, 11.20000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (295, 10358, 36, 15.20000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (296, 10359, 16, 13.90000, 56, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (297, 10359, 31, 10.00000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (298, 10359, 60, 27.20000, 80, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (299, 10360, 28, 36.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (300, 10360, 29, 99.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (301, 10360, 38, 210.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (302, 10360, 49, 16.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (303, 10360, 54, 5.90000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (304, 10361, 39, 14.40000, 54, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (305, 10361, 60, 27.20000, 55, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (306, 10362, 25, 11.20000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (307, 10362, 51, 42.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (308, 10362, 54, 5.90000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (309, 10363, 31, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (310, 10363, 75, 6.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (311, 10363, 76, 14.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (312, 10364, 69, 28.80000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (313, 10364, 71, 17.20000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (314, 10365, 11, 16.80000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (315, 10366, 65, 16.80000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (316, 10366, 77, 10.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (317, 10367, 34, 11.20000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (318, 10367, 54, 5.90000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (319, 10367, 65, 16.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (320, 10367, 77, 10.40000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (321, 10368, 21, 8.00000, 5, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (322, 10368, 28, 36.40000, 13, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (323, 10368, 57, 15.60000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (324, 10368, 64, 26.60000, 35, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (325, 10369, 29, 99.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (326, 10369, 56, 30.40000, 18, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (327, 10370, 1, 14.40000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (328, 10370, 64, 26.60000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (329, 10370, 74, 8.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (330, 10371, 36, 15.20000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (331, 10372, 20, 64.80000, 12, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (332, 10372, 38, 210.80000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (333, 10372, 60, 27.20000, 70, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (334, 10372, 72, 27.80000, 42, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (335, 10373, 58, 10.60000, 80, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (336, 10373, 71, 17.20000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (337, 10374, 31, 10.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (338, 10374, 58, 10.60000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (339, 10375, 14, 18.60000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (340, 10375, 54, 5.90000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (341, 10376, 31, 10.00000, 42, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (342, 10377, 28, 36.40000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (343, 10377, 39, 14.40000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (344, 10378, 71, 17.20000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (345, 10379, 41, 7.70000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (346, 10379, 63, 35.10000, 16, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (347, 10379, 65, 16.80000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (348, 10380, 30, 20.70000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (349, 10380, 53, 26.20000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (350, 10380, 60, 27.20000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (351, 10380, 70, 12.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (352, 10381, 74, 8.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (353, 10382, 5, 17.00000, 32, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (354, 10382, 18, 50.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (355, 10382, 29, 99.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (356, 10382, 33, 2.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (357, 10382, 74, 8.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (358, 10383, 13, 4.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (359, 10383, 50, 13.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (360, 10383, 56, 30.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (361, 10384, 20, 64.80000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (362, 10384, 60, 27.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (363, 10385, 7, 24.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (364, 10385, 60, 27.20000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (365, 10385, 68, 10.00000, 8, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (366, 10386, 24, 3.60000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (367, 10386, 34, 11.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (368, 10387, 24, 3.60000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (369, 10387, 28, 36.40000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (370, 10387, 59, 44.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (371, 10387, 71, 17.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (372, 10388, 45, 7.60000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (373, 10388, 52, 5.60000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (374, 10388, 53, 26.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (375, 10389, 10, 24.80000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (376, 10389, 55, 19.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (377, 10389, 62, 39.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (378, 10389, 70, 12.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (379, 10390, 31, 10.00000, 60, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (380, 10390, 35, 14.40000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (381, 10390, 46, 9.60000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (382, 10390, 72, 27.80000, 24, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (383, 10391, 13, 4.80000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (384, 10392, 69, 28.80000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (385, 10393, 2, 15.20000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (386, 10393, 14, 18.60000, 42, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (387, 10393, 25, 11.20000, 7, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (388, 10393, 26, 24.90000, 70, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (389, 10393, 31, 10.00000, 32, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (390, 10394, 13, 4.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (391, 10394, 62, 39.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (392, 10395, 46, 9.60000, 28, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (393, 10395, 53, 26.20000, 70, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (394, 10395, 69, 28.80000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (395, 10396, 23, 7.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (396, 10396, 71, 17.20000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (397, 10396, 72, 27.80000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (398, 10397, 21, 8.00000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (399, 10397, 51, 42.40000, 18, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (400, 10398, 35, 14.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (401, 10398, 55, 19.20000, 120, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (402, 10399, 68, 10.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (403, 10399, 71, 17.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (404, 10399, 76, 14.40000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (405, 10399, 77, 10.40000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (406, 10400, 29, 99.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (407, 10400, 35, 14.40000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (408, 10400, 49, 16.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (409, 10401, 30, 20.70000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (410, 10401, 56, 30.40000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (411, 10401, 65, 16.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (412, 10401, 71, 17.20000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (413, 10402, 23, 7.20000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (414, 10402, 63, 35.10000, 65, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (415, 10403, 16, 13.90000, 21, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (416, 10403, 48, 10.20000, 70, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (417, 10404, 26, 24.90000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (418, 10404, 42, 11.20000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (419, 10404, 49, 16.00000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (420, 10405, 3, 8.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (421, 10406, 1, 14.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (422, 10406, 21, 8.00000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (423, 10406, 28, 36.40000, 42, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (424, 10406, 36, 15.20000, 5, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (425, 10406, 40, 14.70000, 2, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (426, 10407, 11, 16.80000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (427, 10407, 69, 28.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (428, 10407, 71, 17.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (429, 10408, 37, 20.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (430, 10408, 54, 5.90000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (431, 10408, 62, 39.40000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (432, 10409, 14, 18.60000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (433, 10409, 21, 8.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (434, 10410, 33, 2.00000, 49, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (435, 10410, 59, 44.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (436, 10411, 41, 7.70000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (437, 10411, 44, 15.50000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (438, 10411, 59, 44.00000, 9, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (439, 10412, 14, 18.60000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (440, 10413, 1, 14.40000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (441, 10413, 62, 39.40000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (442, 10413, 76, 14.40000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (443, 10414, 19, 7.30000, 18, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (444, 10414, 33, 2.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (445, 10415, 17, 31.20000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (446, 10415, 33, 2.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (447, 10416, 19, 7.30000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (448, 10416, 53, 26.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (449, 10416, 57, 15.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (450, 10417, 38, 210.80000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (451, 10417, 46, 9.60000, 2, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (452, 10417, 68, 10.00000, 36, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (453, 10417, 77, 10.40000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (454, 10418, 2, 15.20000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (455, 10418, 47, 7.60000, 55, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (456, 10418, 61, 22.80000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (457, 10418, 74, 8.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (458, 10419, 60, 27.20000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (459, 10419, 69, 28.80000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (460, 10420, 9, 77.60000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (461, 10420, 13, 4.80000, 2, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (462, 10420, 70, 12.00000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (463, 10420, 73, 12.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (464, 10421, 19, 7.30000, 4, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (465, 10421, 26, 24.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (466, 10421, 53, 26.20000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (467, 10421, 77, 10.40000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (468, 10422, 26, 24.90000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (469, 10423, 31, 10.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (470, 10423, 59, 44.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (471, 10424, 35, 14.40000, 60, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (472, 10424, 38, 210.80000, 49, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (473, 10424, 68, 10.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (474, 10425, 55, 19.20000, 10, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (475, 10425, 76, 14.40000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (476, 10426, 56, 30.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (477, 10426, 64, 26.60000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (478, 10427, 14, 18.60000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (479, 10428, 46, 9.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (480, 10429, 50, 13.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (481, 10429, 63, 35.10000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (482, 10430, 17, 31.20000, 45, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (483, 10430, 21, 8.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (484, 10430, 56, 30.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (485, 10430, 59, 44.00000, 70, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (486, 10431, 17, 31.20000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (487, 10431, 40, 14.70000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (488, 10431, 47, 7.60000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (489, 10432, 26, 24.90000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (490, 10432, 54, 5.90000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (491, 10433, 56, 30.40000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (492, 10434, 11, 16.80000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (493, 10434, 76, 14.40000, 18, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (494, 10435, 2, 15.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (495, 10435, 22, 16.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (496, 10435, 72, 27.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (497, 10436, 46, 9.60000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (498, 10436, 56, 30.40000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (499, 10436, 64, 26.60000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (500, 10436, 75, 6.20000, 24, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (501, 10437, 53, 26.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (502, 10438, 19, 7.30000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (503, 10438, 34, 11.20000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (504, 10438, 57, 15.60000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (505, 10439, 12, 30.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (506, 10439, 16, 13.90000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (507, 10439, 64, 26.60000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (508, 10439, 74, 8.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (509, 10440, 2, 15.20000, 45, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (510, 10440, 16, 13.90000, 49, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (511, 10440, 29, 99.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (512, 10440, 61, 22.80000, 90, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (513, 10441, 27, 35.10000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (514, 10442, 11, 16.80000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (515, 10442, 54, 5.90000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (516, 10442, 66, 13.60000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (517, 10443, 11, 16.80000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (518, 10443, 28, 36.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (519, 10444, 17, 31.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (520, 10444, 26, 24.90000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (521, 10444, 35, 14.40000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (522, 10444, 41, 7.70000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (523, 10445, 39, 14.40000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (524, 10445, 54, 5.90000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (525, 10446, 19, 7.30000, 12, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (526, 10446, 24, 3.60000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (527, 10446, 31, 10.00000, 3, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (528, 10446, 52, 5.60000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (529, 10447, 19, 7.30000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (530, 10447, 65, 16.80000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (531, 10447, 71, 17.20000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (532, 10448, 26, 24.90000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (533, 10448, 40, 14.70000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (534, 10449, 10, 24.80000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (535, 10449, 52, 5.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (536, 10449, 62, 39.40000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (537, 10450, 10, 24.80000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (538, 10450, 54, 5.90000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (539, 10451, 55, 19.20000, 120, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (540, 10451, 64, 26.60000, 35, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (541, 10451, 65, 16.80000, 28, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (542, 10451, 77, 10.40000, 55, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (543, 10452, 28, 36.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (544, 10452, 44, 15.50000, 100, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (545, 10453, 48, 10.20000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (546, 10453, 70, 12.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (547, 10454, 16, 13.90000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (548, 10454, 33, 2.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (549, 10454, 46, 9.60000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (550, 10455, 39, 14.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (551, 10455, 53, 26.20000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (552, 10455, 61, 22.80000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (553, 10455, 71, 17.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (554, 10456, 21, 8.00000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (555, 10456, 49, 16.00000, 21, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (556, 10457, 59, 44.00000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (557, 10458, 26, 24.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (558, 10458, 28, 36.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (559, 10458, 43, 36.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (560, 10458, 56, 30.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (561, 10458, 71, 17.20000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (562, 10459, 7, 24.00000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (563, 10459, 46, 9.60000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (564, 10459, 72, 27.80000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (565, 10460, 68, 10.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (566, 10460, 75, 6.20000, 4, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (567, 10461, 21, 8.00000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (568, 10461, 30, 20.70000, 28, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (569, 10461, 55, 19.20000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (570, 10462, 13, 4.80000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (571, 10462, 23, 7.20000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (572, 10463, 19, 7.30000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (573, 10463, 42, 11.20000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (574, 10464, 4, 17.60000, 16, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (575, 10464, 43, 36.80000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (576, 10464, 56, 30.40000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (577, 10464, 60, 27.20000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (578, 10465, 24, 3.60000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (579, 10465, 29, 99.00000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (580, 10465, 40, 14.70000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (581, 10465, 45, 7.60000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (582, 10465, 50, 13.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (583, 10466, 11, 16.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (584, 10466, 46, 9.60000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (585, 10467, 24, 3.60000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (586, 10467, 25, 11.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (587, 10468, 30, 20.70000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (588, 10468, 43, 36.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (589, 10469, 2, 15.20000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (590, 10469, 16, 13.90000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (591, 10469, 44, 15.50000, 2, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (592, 10470, 18, 50.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (593, 10470, 23, 7.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (594, 10470, 64, 26.60000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (595, 10471, 7, 24.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (596, 10471, 56, 30.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (597, 10472, 24, 3.60000, 80, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (598, 10472, 51, 42.40000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (599, 10473, 33, 2.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (600, 10473, 71, 17.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (601, 10474, 14, 18.60000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (602, 10474, 28, 36.40000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (603, 10474, 40, 14.70000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (604, 10474, 75, 6.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (605, 10475, 31, 10.00000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (606, 10475, 66, 13.60000, 60, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (607, 10475, 76, 14.40000, 42, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (608, 10476, 55, 19.20000, 2, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (609, 10476, 70, 12.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (610, 10477, 1, 14.40000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (611, 10477, 21, 8.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (612, 10477, 39, 14.40000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (613, 10478, 10, 24.80000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (614, 10479, 38, 210.80000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (615, 10479, 53, 26.20000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (616, 10479, 59, 44.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (617, 10479, 64, 26.60000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (618, 10480, 47, 7.60000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (619, 10480, 59, 44.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (620, 10481, 49, 16.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (621, 10481, 60, 27.20000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (622, 10482, 40, 14.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (623, 10483, 34, 11.20000, 35, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (624, 10483, 77, 10.40000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (625, 10484, 21, 8.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (626, 10484, 40, 14.70000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (627, 10484, 51, 42.40000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (628, 10485, 2, 15.20000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (629, 10485, 3, 8.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (630, 10485, 55, 19.20000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (631, 10485, 70, 12.00000, 60, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (632, 10486, 11, 16.80000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (633, 10486, 51, 42.40000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (634, 10486, 74, 8.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (635, 10487, 19, 7.30000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (636, 10487, 26, 24.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (637, 10487, 54, 5.90000, 24, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (638, 10488, 59, 44.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (639, 10488, 73, 12.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (640, 10489, 11, 16.80000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (641, 10489, 16, 13.90000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (642, 10490, 59, 44.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (643, 10490, 68, 10.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (644, 10490, 75, 6.20000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (645, 10491, 44, 15.50000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (646, 10491, 77, 10.40000, 7, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (647, 10492, 25, 11.20000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (648, 10492, 42, 11.20000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (649, 10493, 65, 16.80000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (650, 10493, 66, 13.60000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (651, 10493, 69, 28.80000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (652, 10494, 56, 30.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (653, 10495, 23, 7.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (654, 10495, 41, 7.70000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (655, 10495, 77, 10.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (656, 10496, 31, 10.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (657, 10497, 56, 30.40000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (658, 10497, 72, 27.80000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (659, 10497, 77, 10.40000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (660, 10498, 24, 4.50000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (661, 10498, 40, 18.40000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (662, 10498, 42, 14.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (663, 10499, 28, 45.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (664, 10499, 49, 20.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (665, 10500, 15, 15.50000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (666, 10500, 28, 45.60000, 8, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (667, 10501, 54, 7.45000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (668, 10502, 45, 9.50000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (669, 10502, 53, 32.80000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (670, 10502, 67, 14.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (671, 10503, 14, 23.25000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (672, 10503, 65, 21.05000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (673, 10504, 2, 19.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (674, 10504, 21, 10.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (675, 10504, 53, 32.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (676, 10504, 61, 28.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (677, 10505, 62, 49.30000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (678, 10506, 25, 14.00000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (679, 10506, 70, 15.00000, 14, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (680, 10507, 43, 46.00000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (681, 10507, 48, 12.75000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (682, 10508, 13, 6.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (683, 10508, 39, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (684, 10509, 28, 45.60000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (685, 10510, 29, 123.79000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (686, 10510, 75, 7.75000, 36, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (687, 10511, 4, 22.00000, 50, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (688, 10511, 7, 30.00000, 50, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (689, 10511, 8, 40.00000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (690, 10512, 24, 4.50000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (691, 10512, 46, 12.00000, 9, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (692, 10512, 47, 9.50000, 6, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (693, 10512, 60, 34.00000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (694, 10513, 21, 10.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (695, 10513, 32, 32.00000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (696, 10513, 61, 28.50000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (697, 10514, 20, 81.00000, 39, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (698, 10514, 28, 45.60000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (699, 10514, 56, 38.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (700, 10514, 65, 21.05000, 39, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (701, 10514, 75, 7.75000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (702, 10515, 9, 97.00000, 16, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (703, 10515, 16, 17.45000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (704, 10515, 27, 43.90000, 120, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (705, 10515, 33, 2.50000, 16, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (706, 10515, 60, 34.00000, 84, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (707, 10516, 18, 62.50000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (708, 10516, 41, 9.65000, 80, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (709, 10516, 42, 14.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (710, 10517, 52, 7.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (711, 10517, 59, 55.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (712, 10517, 70, 15.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (713, 10518, 24, 4.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (714, 10518, 38, 263.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (715, 10518, 44, 19.45000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (716, 10519, 10, 31.00000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (717, 10519, 56, 38.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (718, 10519, 60, 34.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (719, 10520, 24, 4.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (720, 10520, 53, 32.80000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (721, 10521, 35, 18.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (722, 10521, 41, 9.65000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (723, 10521, 68, 12.50000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (724, 10522, 1, 18.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (725, 10522, 8, 40.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (726, 10522, 30, 25.89000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (727, 10522, 40, 18.40000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (728, 10523, 17, 39.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (729, 10523, 20, 81.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (730, 10523, 37, 26.00000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (731, 10523, 41, 9.65000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (732, 10524, 10, 31.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (733, 10524, 30, 25.89000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (734, 10524, 43, 46.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (735, 10524, 54, 7.45000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (736, 10525, 36, 19.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (737, 10525, 40, 18.40000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (738, 10526, 1, 18.00000, 8, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (739, 10526, 13, 6.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (740, 10526, 56, 38.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (741, 10527, 4, 22.00000, 50, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (742, 10527, 36, 19.00000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (743, 10528, 11, 21.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (744, 10528, 33, 2.50000, 8, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (745, 10528, 72, 34.80000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (746, 10529, 55, 24.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (747, 10529, 68, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (748, 10529, 69, 36.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (749, 10530, 17, 39.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (750, 10530, 43, 46.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (751, 10530, 61, 28.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (752, 10530, 76, 18.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (753, 10531, 59, 55.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (754, 10532, 30, 25.89000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (755, 10532, 66, 17.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (756, 10533, 4, 22.00000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (757, 10533, 72, 34.80000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (758, 10533, 73, 15.00000, 24, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (759, 10534, 30, 25.89000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (760, 10534, 40, 18.40000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (761, 10534, 54, 7.45000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (762, 10535, 11, 21.00000, 50, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (763, 10535, 40, 18.40000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (764, 10535, 57, 19.50000, 5, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (765, 10535, 59, 55.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (766, 10536, 12, 38.00000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (767, 10536, 31, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (768, 10536, 33, 2.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (769, 10536, 60, 34.00000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (770, 10537, 31, 12.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (771, 10537, 51, 53.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (772, 10537, 58, 13.25000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (773, 10537, 72, 34.80000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (774, 10537, 73, 15.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (775, 10538, 70, 15.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (776, 10538, 72, 34.80000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (777, 10539, 13, 6.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (778, 10539, 21, 10.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (779, 10539, 33, 2.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (780, 10539, 49, 20.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (781, 10540, 3, 10.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (782, 10540, 26, 31.23000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (783, 10540, 38, 263.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (784, 10540, 68, 12.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (785, 10541, 24, 4.50000, 35, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (786, 10541, 38, 263.50000, 4, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (787, 10541, 65, 21.05000, 36, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (788, 10541, 71, 21.50000, 9, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (789, 10542, 11, 21.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (790, 10542, 54, 7.45000, 24, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (791, 10543, 12, 38.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (792, 10543, 23, 9.00000, 70, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (793, 10544, 28, 45.60000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (794, 10544, 67, 14.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (795, 10545, 11, 21.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (796, 10546, 7, 30.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (797, 10546, 35, 18.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (798, 10546, 62, 49.30000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (799, 10547, 32, 32.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (800, 10547, 36, 19.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (801, 10548, 34, 14.00000, 10, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (802, 10548, 41, 9.65000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (803, 10549, 31, 12.50000, 55, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (804, 10549, 45, 9.50000, 100, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (805, 10549, 51, 53.00000, 48, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (806, 10550, 17, 39.00000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (807, 10550, 19, 9.20000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (808, 10550, 21, 10.00000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (809, 10550, 61, 28.50000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (810, 10551, 16, 17.45000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (811, 10551, 35, 18.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (812, 10551, 44, 19.45000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (813, 10552, 69, 36.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (814, 10552, 75, 7.75000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (815, 10553, 11, 21.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (816, 10553, 16, 17.45000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (817, 10553, 22, 21.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (818, 10553, 31, 12.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (819, 10553, 35, 18.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (820, 10554, 16, 17.45000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (821, 10554, 23, 9.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (822, 10554, 62, 49.30000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (823, 10554, 77, 13.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (824, 10555, 14, 23.25000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (825, 10555, 19, 9.20000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (826, 10555, 24, 4.50000, 18, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (827, 10555, 51, 53.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (828, 10555, 56, 38.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (829, 10556, 72, 34.80000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (830, 10557, 64, 33.25000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (831, 10557, 75, 7.75000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (832, 10558, 47, 9.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (833, 10558, 51, 53.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (834, 10558, 52, 7.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (835, 10558, 53, 32.80000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (836, 10558, 73, 15.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (837, 10559, 41, 9.65000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (838, 10559, 55, 24.00000, 18, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (839, 10560, 30, 25.89000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (840, 10560, 62, 49.30000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (841, 10561, 44, 19.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (842, 10561, 51, 53.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (843, 10562, 33, 2.50000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (844, 10562, 62, 49.30000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (845, 10563, 36, 19.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (846, 10563, 52, 7.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (847, 10564, 17, 39.00000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (848, 10564, 31, 12.50000, 6, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (849, 10564, 55, 24.00000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (850, 10565, 24, 4.50000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (851, 10565, 64, 33.25000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (852, 10566, 11, 21.00000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (853, 10566, 18, 62.50000, 18, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (854, 10566, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (855, 10567, 31, 12.50000, 60, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (856, 10567, 51, 53.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (857, 10567, 59, 55.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (858, 10568, 10, 31.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (859, 10569, 31, 12.50000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (860, 10569, 76, 18.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (861, 10570, 11, 21.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (862, 10570, 56, 38.00000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (863, 10571, 14, 23.25000, 11, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (864, 10571, 42, 14.00000, 28, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (865, 10572, 16, 17.45000, 12, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (866, 10572, 32, 32.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (867, 10572, 40, 18.40000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (868, 10572, 75, 7.75000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (869, 10573, 17, 39.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (870, 10573, 34, 14.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (871, 10573, 53, 32.80000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (872, 10574, 33, 2.50000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (873, 10574, 40, 18.40000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (874, 10574, 62, 49.30000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (875, 10574, 64, 33.25000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (876, 10575, 59, 55.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (877, 10575, 63, 43.90000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (878, 10575, 72, 34.80000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (879, 10575, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (880, 10576, 1, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (881, 10576, 31, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (882, 10576, 44, 19.45000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (883, 10577, 39, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (884, 10577, 75, 7.75000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (885, 10577, 77, 13.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (886, 10578, 35, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (887, 10578, 57, 19.50000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (888, 10579, 15, 15.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (889, 10579, 75, 7.75000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (890, 10580, 14, 23.25000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (891, 10580, 41, 9.65000, 9, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (892, 10580, 65, 21.05000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (893, 10581, 75, 7.75000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (894, 10582, 57, 19.50000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (895, 10582, 76, 18.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (896, 10583, 29, 123.79000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (897, 10583, 60, 34.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (898, 10583, 69, 36.00000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (899, 10584, 31, 12.50000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (900, 10585, 47, 9.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (901, 10586, 52, 7.00000, 4, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (902, 10587, 26, 31.23000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (903, 10587, 35, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (904, 10587, 77, 13.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (905, 10588, 18, 62.50000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (906, 10588, 42, 14.00000, 100, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (907, 10589, 35, 18.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (908, 10590, 1, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (909, 10590, 77, 13.00000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (910, 10591, 3, 10.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (911, 10591, 7, 30.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (912, 10591, 54, 7.45000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (913, 10592, 15, 15.50000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (914, 10592, 26, 31.23000, 5, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (915, 10593, 20, 81.00000, 21, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (916, 10593, 69, 36.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (917, 10593, 76, 18.00000, 4, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (918, 10594, 52, 7.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (919, 10594, 58, 13.25000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (920, 10595, 35, 18.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (921, 10595, 61, 28.50000, 120, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (922, 10595, 69, 36.00000, 65, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (923, 10596, 56, 38.00000, 5, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (924, 10596, 63, 43.90000, 24, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (925, 10596, 75, 7.75000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (926, 10597, 24, 4.50000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (927, 10597, 57, 19.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (928, 10597, 65, 21.05000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (929, 10598, 27, 43.90000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (930, 10598, 71, 21.50000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (931, 10599, 62, 49.30000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (932, 10600, 54, 7.45000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (933, 10600, 73, 15.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (934, 10601, 13, 6.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (935, 10601, 59, 55.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (936, 10602, 77, 13.00000, 5, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (937, 10603, 22, 21.00000, 48, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (938, 10603, 49, 20.00000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (939, 10604, 48, 12.75000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (940, 10604, 76, 18.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (941, 10605, 16, 17.45000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (942, 10605, 59, 55.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (943, 10605, 60, 34.00000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (944, 10605, 71, 21.50000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (945, 10606, 4, 22.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (946, 10606, 55, 24.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (947, 10606, 62, 49.30000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (948, 10607, 7, 30.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (949, 10607, 17, 39.00000, 100, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (950, 10607, 33, 2.50000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (951, 10607, 40, 18.40000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (952, 10607, 72, 34.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (953, 10608, 56, 38.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (954, 10609, 1, 18.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (955, 10609, 10, 31.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (956, 10609, 21, 10.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (957, 10610, 36, 19.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (958, 10611, 1, 18.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (959, 10611, 2, 19.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (960, 10611, 60, 34.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (961, 10612, 10, 31.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (962, 10612, 36, 19.00000, 55, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (963, 10612, 49, 20.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (964, 10612, 60, 34.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (965, 10612, 76, 18.00000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (966, 10613, 13, 6.00000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (967, 10613, 75, 7.75000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (968, 10614, 11, 21.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (969, 10614, 21, 10.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (970, 10614, 39, 18.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (971, 10615, 55, 24.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (972, 10616, 38, 263.50000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (973, 10616, 56, 38.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (974, 10616, 70, 15.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (975, 10616, 71, 21.50000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (976, 10617, 59, 55.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (977, 10618, 6, 25.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (978, 10618, 56, 38.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (979, 10618, 68, 12.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (980, 10619, 21, 10.00000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (981, 10619, 22, 21.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (982, 10620, 24, 4.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (983, 10620, 52, 7.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (984, 10621, 19, 9.20000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (985, 10621, 23, 9.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (986, 10621, 70, 15.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (987, 10621, 71, 21.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (988, 10622, 2, 19.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (989, 10622, 68, 12.50000, 18, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (990, 10623, 14, 23.25000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (991, 10623, 19, 9.20000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (992, 10623, 21, 10.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (993, 10623, 24, 4.50000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (994, 10623, 35, 18.00000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (995, 10624, 28, 45.60000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (996, 10624, 29, 123.79000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (997, 10624, 44, 19.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (998, 10625, 14, 23.25000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (999, 10625, 42, 14.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1000, 10625, 60, 34.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1001, 10626, 53, 32.80000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1002, 10626, 60, 34.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1003, 10626, 71, 21.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1004, 10627, 62, 49.30000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1005, 10627, 73, 15.00000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1006, 10628, 1, 18.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1007, 10629, 29, 123.79000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1008, 10629, 64, 33.25000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1009, 10630, 55, 24.00000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1010, 10630, 76, 18.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1011, 10631, 75, 7.75000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1012, 10632, 2, 19.00000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1013, 10632, 33, 2.50000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1014, 10633, 12, 38.00000, 36, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1015, 10633, 13, 6.00000, 13, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1016, 10633, 26, 31.23000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1017, 10633, 62, 49.30000, 80, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1018, 10634, 7, 30.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1019, 10634, 18, 62.50000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1020, 10634, 51, 53.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1021, 10634, 75, 7.75000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1022, 10635, 4, 22.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1023, 10635, 5, 21.35000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1024, 10635, 22, 21.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1025, 10636, 4, 22.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1026, 10636, 58, 13.25000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1027, 10637, 11, 21.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1028, 10637, 50, 16.25000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1029, 10637, 56, 38.00000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1030, 10638, 45, 9.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1031, 10638, 65, 21.05000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1032, 10638, 72, 34.80000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1033, 10639, 18, 62.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1034, 10640, 69, 36.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1035, 10640, 70, 15.00000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1036, 10641, 2, 19.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1037, 10641, 40, 18.40000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1038, 10642, 21, 10.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1039, 10642, 61, 28.50000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1040, 10643, 28, 45.60000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1041, 10643, 39, 18.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1042, 10643, 46, 12.00000, 2, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1043, 10644, 18, 62.50000, 4, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1044, 10644, 43, 46.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1045, 10644, 46, 12.00000, 21, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1046, 10645, 18, 62.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1047, 10645, 36, 19.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1048, 10646, 1, 18.00000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1049, 10646, 10, 31.00000, 18, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1050, 10646, 71, 21.50000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1051, 10646, 77, 13.00000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1052, 10647, 19, 9.20000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1053, 10647, 39, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1054, 10648, 22, 21.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1055, 10648, 24, 4.50000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1056, 10649, 28, 45.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1057, 10649, 72, 34.80000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1058, 10650, 30, 25.89000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1059, 10650, 53, 32.80000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1060, 10650, 54, 7.45000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1061, 10651, 19, 9.20000, 12, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1062, 10651, 22, 21.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1063, 10652, 30, 25.89000, 2, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1064, 10652, 42, 14.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1065, 10653, 16, 17.45000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1066, 10653, 60, 34.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1067, 10654, 4, 22.00000, 12, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1068, 10654, 39, 18.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1069, 10654, 54, 7.45000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1070, 10655, 41, 9.65000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1071, 10656, 14, 23.25000, 3, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1072, 10656, 44, 19.45000, 28, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1073, 10656, 47, 9.50000, 6, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1074, 10657, 15, 15.50000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1075, 10657, 41, 9.65000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1076, 10657, 46, 12.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1077, 10657, 47, 9.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1078, 10657, 56, 38.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1079, 10657, 60, 34.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1080, 10658, 21, 10.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1081, 10658, 40, 18.40000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1082, 10658, 60, 34.00000, 55, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1083, 10658, 77, 13.00000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1084, 10659, 31, 12.50000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1085, 10659, 40, 18.40000, 24, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1086, 10659, 70, 15.00000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1087, 10660, 20, 81.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1088, 10661, 39, 18.00000, 3, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1089, 10661, 58, 13.25000, 49, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1090, 10662, 68, 12.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1091, 10663, 40, 18.40000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1092, 10663, 42, 14.00000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1093, 10663, 51, 53.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1094, 10664, 10, 31.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1095, 10664, 56, 38.00000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1096, 10664, 65, 21.05000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1097, 10665, 51, 53.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1098, 10665, 59, 55.00000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1099, 10665, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1100, 10666, 29, 123.79000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1101, 10666, 65, 21.05000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1102, 10667, 69, 36.00000, 45, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1103, 10667, 71, 21.50000, 14, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1104, 10668, 31, 12.50000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1105, 10668, 55, 24.00000, 4, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1106, 10668, 64, 33.25000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1107, 10669, 36, 19.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1108, 10670, 23, 9.00000, 32, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1109, 10670, 46, 12.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1110, 10670, 67, 14.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1111, 10670, 73, 15.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1112, 10670, 75, 7.75000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1113, 10671, 16, 17.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1114, 10671, 62, 49.30000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1115, 10671, 65, 21.05000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1116, 10672, 38, 263.50000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1117, 10672, 71, 21.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1118, 10673, 16, 17.45000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1119, 10673, 42, 14.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1120, 10673, 43, 46.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1121, 10674, 23, 9.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1122, 10675, 14, 23.25000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1123, 10675, 53, 32.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1124, 10675, 58, 13.25000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1125, 10676, 10, 31.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1126, 10676, 19, 9.20000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1127, 10676, 44, 19.45000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1128, 10677, 26, 31.23000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1129, 10677, 33, 2.50000, 8, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1130, 10678, 12, 38.00000, 100, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1131, 10678, 33, 2.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1132, 10678, 41, 9.65000, 120, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1133, 10678, 54, 7.45000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1134, 10679, 59, 55.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1135, 10680, 16, 17.45000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1136, 10680, 31, 12.50000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1137, 10680, 42, 14.00000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1138, 10681, 19, 9.20000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1139, 10681, 21, 10.00000, 12, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1140, 10681, 64, 33.25000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1141, 10682, 33, 2.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1142, 10682, 66, 17.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1143, 10682, 75, 7.75000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1144, 10683, 52, 7.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1145, 10684, 40, 18.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1146, 10684, 47, 9.50000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1147, 10684, 60, 34.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1148, 10685, 10, 31.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1149, 10685, 41, 9.65000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1150, 10685, 47, 9.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1151, 10686, 17, 39.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1152, 10686, 26, 31.23000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1153, 10687, 9, 97.00000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1154, 10687, 29, 123.79000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1155, 10687, 36, 19.00000, 6, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1156, 10688, 10, 31.00000, 18, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1157, 10688, 28, 45.60000, 60, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1158, 10688, 34, 14.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1159, 10689, 1, 18.00000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1160, 10690, 56, 38.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1161, 10690, 77, 13.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1162, 10691, 1, 18.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1163, 10691, 29, 123.79000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1164, 10691, 43, 46.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1165, 10691, 44, 19.45000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1166, 10691, 62, 49.30000, 48, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1167, 10692, 63, 43.90000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1168, 10693, 9, 97.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1169, 10693, 54, 7.45000, 60, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1170, 10693, 69, 36.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1171, 10693, 73, 15.00000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1172, 10694, 7, 30.00000, 90, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1173, 10694, 59, 55.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1174, 10694, 70, 15.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1175, 10695, 8, 40.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1176, 10695, 12, 38.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1177, 10695, 24, 4.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1178, 10696, 17, 39.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1179, 10696, 46, 12.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1180, 10697, 19, 9.20000, 7, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1181, 10697, 35, 18.00000, 9, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1182, 10697, 58, 13.25000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1183, 10697, 70, 15.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1184, 10698, 11, 21.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1185, 10698, 17, 39.00000, 8, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1186, 10698, 29, 123.79000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1187, 10698, 65, 21.05000, 65, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1188, 10698, 70, 15.00000, 8, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1189, 10699, 47, 9.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1190, 10700, 1, 18.00000, 5, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1191, 10700, 34, 14.00000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1192, 10700, 68, 12.50000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1193, 10700, 71, 21.50000, 60, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1194, 10701, 59, 55.00000, 42, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1195, 10701, 71, 21.50000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1196, 10701, 76, 18.00000, 35, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1197, 10702, 3, 10.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1198, 10702, 76, 18.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1199, 10703, 2, 19.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1200, 10703, 59, 55.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1201, 10703, 73, 15.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1202, 10704, 4, 22.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1203, 10704, 24, 4.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1204, 10704, 48, 12.75000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1205, 10705, 31, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1206, 10705, 32, 32.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1207, 10706, 16, 17.45000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1208, 10706, 43, 46.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1209, 10706, 59, 55.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1210, 10707, 55, 24.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1211, 10707, 57, 19.50000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1212, 10707, 70, 15.00000, 28, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1213, 10708, 5, 21.35000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1214, 10708, 36, 19.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1215, 10709, 8, 40.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1216, 10709, 51, 53.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1217, 10709, 60, 34.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1218, 10710, 19, 9.20000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1219, 10710, 47, 9.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1220, 10711, 19, 9.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1221, 10711, 41, 9.65000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1222, 10711, 53, 32.80000, 120, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1223, 10712, 53, 32.80000, 3, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1224, 10712, 56, 38.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1225, 10713, 10, 31.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1226, 10713, 26, 31.23000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1227, 10713, 45, 9.50000, 110, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1228, 10713, 46, 12.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1229, 10714, 2, 19.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1230, 10714, 17, 39.00000, 27, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1231, 10714, 47, 9.50000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1232, 10714, 56, 38.00000, 18, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1233, 10714, 58, 13.25000, 12, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1234, 10715, 10, 31.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1235, 10715, 71, 21.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1236, 10716, 21, 10.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1237, 10716, 51, 53.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1238, 10716, 61, 28.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1239, 10717, 21, 10.00000, 32, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1240, 10717, 54, 7.45000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1241, 10717, 69, 36.00000, 25, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1242, 10718, 12, 38.00000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1243, 10718, 16, 17.45000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1244, 10718, 36, 19.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1245, 10718, 62, 49.30000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1246, 10719, 18, 62.50000, 12, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1247, 10719, 30, 25.89000, 3, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1248, 10719, 54, 7.45000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1249, 10720, 35, 18.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1250, 10720, 71, 21.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1251, 10721, 44, 19.45000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1252, 10722, 2, 19.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1253, 10722, 31, 12.50000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1254, 10722, 68, 12.50000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1255, 10722, 75, 7.75000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1256, 10723, 26, 31.23000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1257, 10724, 10, 31.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1258, 10724, 61, 28.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1259, 10725, 41, 9.65000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1260, 10725, 52, 7.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1261, 10725, 55, 24.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1262, 10726, 4, 22.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1263, 10726, 11, 21.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1264, 10727, 17, 39.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1265, 10727, 56, 38.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1266, 10727, 59, 55.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1267, 10728, 30, 25.89000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1268, 10728, 40, 18.40000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1269, 10728, 55, 24.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1270, 10728, 60, 34.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1271, 10729, 1, 18.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1272, 10729, 21, 10.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1273, 10729, 50, 16.25000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1274, 10730, 16, 17.45000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1275, 10730, 31, 12.50000, 3, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1276, 10730, 65, 21.05000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1277, 10731, 21, 10.00000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1278, 10731, 51, 53.00000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1279, 10732, 76, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1280, 10733, 14, 23.25000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1281, 10733, 28, 45.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1282, 10733, 52, 7.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1283, 10734, 6, 25.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1284, 10734, 30, 25.89000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1285, 10734, 76, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1286, 10735, 61, 28.50000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1287, 10735, 77, 13.00000, 2, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1288, 10736, 65, 21.05000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1289, 10736, 75, 7.75000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1290, 10737, 13, 6.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1291, 10737, 41, 9.65000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1292, 10738, 16, 17.45000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1293, 10739, 36, 19.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1294, 10739, 52, 7.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1295, 10740, 28, 45.60000, 5, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1296, 10740, 35, 18.00000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1297, 10740, 45, 9.50000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1298, 10740, 56, 38.00000, 14, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1299, 10741, 2, 19.00000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1300, 10742, 3, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1301, 10742, 60, 34.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1302, 10742, 72, 34.80000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1303, 10743, 46, 12.00000, 28, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1304, 10744, 40, 18.40000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1305, 10745, 18, 62.50000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1306, 10745, 44, 19.45000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1307, 10745, 59, 55.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1308, 10745, 72, 34.80000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1309, 10746, 13, 6.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1310, 10746, 42, 14.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1311, 10746, 62, 49.30000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1312, 10746, 69, 36.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1313, 10747, 31, 12.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1314, 10747, 41, 9.65000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1315, 10747, 63, 43.90000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1316, 10747, 69, 36.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1317, 10748, 23, 9.00000, 44, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1318, 10748, 40, 18.40000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1319, 10748, 56, 38.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1320, 10749, 56, 38.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1321, 10749, 59, 55.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1322, 10749, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1323, 10750, 14, 23.25000, 5, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1324, 10750, 45, 9.50000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1325, 10750, 59, 55.00000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1326, 10751, 26, 31.23000, 12, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1327, 10751, 30, 25.89000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1328, 10751, 50, 16.25000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1329, 10751, 73, 15.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1330, 10752, 1, 18.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1331, 10752, 69, 36.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1332, 10753, 45, 9.50000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1333, 10753, 74, 10.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1334, 10754, 40, 18.40000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1335, 10755, 47, 9.50000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1336, 10755, 56, 38.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1337, 10755, 57, 19.50000, 14, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1338, 10755, 69, 36.00000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1339, 10756, 18, 62.50000, 21, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1340, 10756, 36, 19.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1341, 10756, 68, 12.50000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1342, 10756, 69, 36.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1343, 10757, 34, 14.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1344, 10757, 59, 55.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1345, 10757, 62, 49.30000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1346, 10757, 64, 33.25000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1347, 10758, 26, 31.23000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1348, 10758, 52, 7.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1349, 10758, 70, 15.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1350, 10759, 32, 32.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1351, 10760, 25, 14.00000, 12, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1352, 10760, 27, 43.90000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1353, 10760, 43, 46.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1354, 10761, 25, 14.00000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1355, 10761, 75, 7.75000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1356, 10762, 39, 18.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1357, 10762, 47, 9.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1358, 10762, 51, 53.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1359, 10762, 56, 38.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1360, 10763, 21, 10.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1361, 10763, 22, 21.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1362, 10763, 24, 4.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1363, 10764, 3, 10.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1364, 10764, 39, 18.00000, 130, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1365, 10765, 65, 21.05000, 80, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1366, 10766, 2, 19.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1367, 10766, 7, 30.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1368, 10766, 68, 12.50000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1369, 10767, 42, 14.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1370, 10768, 22, 21.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1371, 10768, 31, 12.50000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1372, 10768, 60, 34.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1373, 10768, 71, 21.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1374, 10769, 41, 9.65000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1375, 10769, 52, 7.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1376, 10769, 61, 28.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1377, 10769, 62, 49.30000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1378, 10770, 11, 21.00000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1379, 10771, 71, 21.50000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1380, 10772, 29, 123.79000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1381, 10772, 59, 55.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1382, 10773, 17, 39.00000, 33, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1383, 10773, 31, 12.50000, 70, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1384, 10773, 75, 7.75000, 7, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1385, 10774, 31, 12.50000, 2, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1386, 10774, 66, 17.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1387, 10775, 10, 31.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1388, 10775, 67, 14.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1389, 10776, 31, 12.50000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1390, 10776, 42, 14.00000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1391, 10776, 45, 9.50000, 27, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1392, 10776, 51, 53.00000, 120, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1393, 10777, 42, 14.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1394, 10778, 41, 9.65000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1395, 10779, 16, 17.45000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1396, 10779, 62, 49.30000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1397, 10780, 70, 15.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1398, 10780, 77, 13.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1399, 10781, 54, 7.45000, 3, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1400, 10781, 56, 38.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1401, 10781, 74, 10.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1402, 10782, 31, 12.50000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1403, 10783, 31, 12.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1404, 10783, 38, 263.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1405, 10784, 36, 19.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1406, 10784, 39, 18.00000, 2, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1407, 10784, 72, 34.80000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1408, 10785, 10, 31.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1409, 10785, 75, 7.75000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1410, 10786, 8, 40.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1411, 10786, 30, 25.89000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1412, 10786, 75, 7.75000, 42, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1413, 10787, 2, 19.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1414, 10787, 29, 123.79000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1415, 10788, 19, 9.20000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1416, 10788, 75, 7.75000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1417, 10789, 18, 62.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1418, 10789, 35, 18.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1419, 10789, 63, 43.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1420, 10789, 68, 12.50000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1421, 10790, 7, 30.00000, 3, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1422, 10790, 56, 38.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1423, 10791, 29, 123.79000, 14, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1424, 10791, 41, 9.65000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1425, 10792, 2, 19.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1426, 10792, 54, 7.45000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1427, 10792, 68, 12.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1428, 10793, 41, 9.65000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1429, 10793, 52, 7.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1430, 10794, 14, 23.25000, 15, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1431, 10794, 54, 7.45000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1432, 10795, 16, 17.45000, 65, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1433, 10795, 17, 39.00000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1434, 10796, 26, 31.23000, 21, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1435, 10796, 44, 19.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1436, 10796, 64, 33.25000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1437, 10796, 69, 36.00000, 24, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1438, 10797, 11, 21.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1439, 10798, 62, 49.30000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1440, 10798, 72, 34.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1441, 10799, 13, 6.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1442, 10799, 24, 4.50000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1443, 10799, 59, 55.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1444, 10800, 11, 21.00000, 50, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1445, 10800, 51, 53.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1446, 10800, 54, 7.45000, 7, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1447, 10801, 17, 39.00000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1448, 10801, 29, 123.79000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1449, 10802, 30, 25.89000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1450, 10802, 51, 53.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1451, 10802, 55, 24.00000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1452, 10802, 62, 49.30000, 5, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1453, 10803, 19, 9.20000, 24, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1454, 10803, 25, 14.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1455, 10803, 59, 55.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1456, 10804, 10, 31.00000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1457, 10804, 28, 45.60000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1458, 10804, 49, 20.00000, 4, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1459, 10805, 34, 14.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1460, 10805, 38, 263.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1461, 10806, 2, 19.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1462, 10806, 65, 21.05000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1463, 10806, 74, 10.00000, 15, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1464, 10807, 40, 18.40000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1465, 10808, 56, 38.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1466, 10808, 76, 18.00000, 50, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1467, 10809, 52, 7.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1468, 10810, 13, 6.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1469, 10810, 25, 14.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1470, 10810, 70, 15.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1471, 10811, 19, 9.20000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1472, 10811, 23, 9.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1473, 10811, 40, 18.40000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1474, 10812, 31, 12.50000, 16, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1475, 10812, 72, 34.80000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1476, 10812, 77, 13.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1477, 10813, 2, 19.00000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1478, 10813, 46, 12.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1479, 10814, 41, 9.65000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1480, 10814, 43, 46.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1481, 10814, 48, 12.75000, 8, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1482, 10814, 61, 28.50000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1483, 10815, 33, 2.50000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1484, 10816, 38, 263.50000, 30, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1485, 10816, 62, 49.30000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1486, 10817, 26, 31.23000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1487, 10817, 38, 263.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1488, 10817, 40, 18.40000, 60, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1489, 10817, 62, 49.30000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1490, 10818, 32, 32.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1491, 10818, 41, 9.65000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1492, 10819, 43, 46.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1493, 10819, 75, 7.75000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1494, 10820, 56, 38.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1495, 10821, 35, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1496, 10821, 51, 53.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1497, 10822, 62, 49.30000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1498, 10822, 70, 15.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1499, 10823, 11, 21.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1500, 10823, 57, 19.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1501, 10823, 59, 55.00000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1502, 10823, 77, 13.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1503, 10824, 41, 9.65000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1504, 10824, 70, 15.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1505, 10825, 26, 31.23000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1506, 10825, 53, 32.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1507, 10826, 31, 12.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1508, 10826, 57, 19.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1509, 10827, 10, 31.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1510, 10827, 39, 18.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1511, 10828, 20, 81.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1512, 10828, 38, 263.50000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1513, 10829, 2, 19.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1514, 10829, 8, 40.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1515, 10829, 13, 6.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1516, 10829, 60, 34.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1517, 10830, 6, 25.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1518, 10830, 39, 18.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1519, 10830, 60, 34.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1520, 10830, 68, 12.50000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1521, 10831, 19, 9.20000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1522, 10831, 35, 18.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1523, 10831, 38, 263.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1524, 10831, 43, 46.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1525, 10832, 13, 6.00000, 3, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1526, 10832, 25, 14.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1527, 10832, 44, 19.45000, 16, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1528, 10832, 64, 33.25000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1529, 10833, 7, 30.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1530, 10833, 31, 12.50000, 9, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1531, 10833, 53, 32.80000, 9, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1532, 10834, 29, 123.79000, 8, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1533, 10834, 30, 25.89000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1534, 10835, 59, 55.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1535, 10835, 77, 13.00000, 2, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1536, 10836, 22, 21.00000, 52, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1537, 10836, 35, 18.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1538, 10836, 57, 19.50000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1539, 10836, 60, 34.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1540, 10836, 64, 33.25000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1541, 10837, 13, 6.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1542, 10837, 40, 18.40000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1543, 10837, 47, 9.50000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1544, 10837, 76, 18.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1545, 10838, 1, 18.00000, 4, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1546, 10838, 18, 62.50000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1547, 10838, 36, 19.00000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1548, 10839, 58, 13.25000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1549, 10839, 72, 34.80000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1550, 10840, 25, 14.00000, 6, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1551, 10840, 39, 18.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1552, 10841, 10, 31.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1553, 10841, 56, 38.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1554, 10841, 59, 55.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1555, 10841, 77, 13.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1556, 10842, 11, 21.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1557, 10842, 43, 46.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1558, 10842, 68, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1559, 10842, 70, 15.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1560, 10843, 51, 53.00000, 4, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1561, 10844, 22, 21.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1562, 10845, 23, 9.00000, 70, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1563, 10845, 35, 18.00000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1564, 10845, 42, 14.00000, 42, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1565, 10845, 58, 13.25000, 60, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1566, 10845, 64, 33.25000, 48, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1567, 10846, 4, 22.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1568, 10846, 70, 15.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1569, 10846, 74, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1570, 10847, 1, 18.00000, 80, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1571, 10847, 19, 9.20000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1572, 10847, 37, 26.00000, 60, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1573, 10847, 45, 9.50000, 36, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1574, 10847, 60, 34.00000, 45, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1575, 10847, 71, 21.50000, 55, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1576, 10848, 5, 21.35000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1577, 10848, 9, 97.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1578, 10849, 3, 10.00000, 49, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1579, 10849, 26, 31.23000, 18, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1580, 10850, 25, 14.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1581, 10850, 33, 2.50000, 4, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1582, 10850, 70, 15.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1583, 10851, 2, 19.00000, 5, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1584, 10851, 25, 14.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1585, 10851, 57, 19.50000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1586, 10851, 59, 55.00000, 42, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1587, 10852, 2, 19.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1588, 10852, 17, 39.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1589, 10852, 62, 49.30000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1590, 10853, 18, 62.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1591, 10854, 10, 31.00000, 100, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1592, 10854, 13, 6.00000, 65, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1593, 10855, 16, 17.45000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1594, 10855, 31, 12.50000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1595, 10855, 56, 38.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1596, 10855, 65, 21.05000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1597, 10856, 2, 19.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1598, 10856, 42, 14.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1599, 10857, 3, 10.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1600, 10857, 26, 31.23000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1601, 10857, 29, 123.79000, 10, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1602, 10858, 7, 30.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1603, 10858, 27, 43.90000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1604, 10858, 70, 15.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1605, 10859, 24, 4.50000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1606, 10859, 54, 7.45000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1607, 10859, 64, 33.25000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1608, 10860, 51, 53.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1609, 10860, 76, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1610, 10861, 17, 39.00000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1611, 10861, 18, 62.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1612, 10861, 21, 10.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1613, 10861, 33, 2.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1614, 10861, 62, 49.30000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1615, 10862, 11, 21.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1616, 10862, 52, 7.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1617, 10863, 1, 18.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1618, 10863, 58, 13.25000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1619, 10864, 35, 18.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1620, 10864, 67, 14.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1621, 10865, 38, 263.50000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1622, 10865, 39, 18.00000, 80, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1623, 10866, 2, 19.00000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1624, 10866, 24, 4.50000, 6, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1625, 10866, 30, 25.89000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1626, 10867, 53, 32.80000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1627, 10868, 26, 31.23000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1628, 10868, 35, 18.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1629, 10868, 49, 20.00000, 42, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1630, 10869, 1, 18.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1631, 10869, 11, 21.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1632, 10869, 23, 9.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1633, 10869, 68, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1634, 10870, 35, 18.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1635, 10870, 51, 53.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1636, 10871, 6, 25.00000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1637, 10871, 16, 17.45000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1638, 10871, 17, 39.00000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1639, 10872, 55, 24.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1640, 10872, 62, 49.30000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1641, 10872, 64, 33.25000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1642, 10872, 65, 21.05000, 21, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1643, 10873, 21, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1644, 10873, 28, 45.60000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1645, 10874, 10, 31.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1646, 10875, 19, 9.20000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1647, 10875, 47, 9.50000, 21, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1648, 10875, 49, 20.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1649, 10876, 46, 12.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1650, 10876, 64, 33.25000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1651, 10877, 16, 17.45000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1652, 10877, 18, 62.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1653, 10878, 20, 81.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1654, 10879, 40, 18.40000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1655, 10879, 65, 21.05000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1656, 10879, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1657, 10880, 23, 9.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1658, 10880, 61, 28.50000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1659, 10880, 70, 15.00000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1660, 10881, 73, 15.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1661, 10882, 42, 14.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1662, 10882, 49, 20.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1663, 10882, 54, 7.45000, 32, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1664, 10883, 24, 4.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1665, 10884, 21, 10.00000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1666, 10884, 56, 38.00000, 21, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1667, 10884, 65, 21.05000, 12, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1668, 10885, 2, 19.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1669, 10885, 24, 4.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1670, 10885, 70, 15.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1671, 10885, 77, 13.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1672, 10886, 10, 31.00000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1673, 10886, 31, 12.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1674, 10886, 77, 13.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1675, 10887, 25, 14.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1676, 10888, 2, 19.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1677, 10888, 68, 12.50000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1678, 10889, 11, 21.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1679, 10889, 38, 263.50000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1680, 10890, 17, 39.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1681, 10890, 34, 14.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1682, 10890, 41, 9.65000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1683, 10891, 30, 25.89000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1684, 10892, 59, 55.00000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1685, 10893, 8, 40.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1686, 10893, 24, 4.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1687, 10893, 29, 123.79000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1688, 10893, 30, 25.89000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1689, 10893, 36, 19.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1690, 10894, 13, 6.00000, 28, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1691, 10894, 69, 36.00000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1692, 10894, 75, 7.75000, 120, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1693, 10895, 24, 4.50000, 110, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1694, 10895, 39, 18.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1695, 10895, 40, 18.40000, 91, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1696, 10895, 60, 34.00000, 100, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1697, 10896, 45, 9.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1698, 10896, 56, 38.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1699, 10897, 29, 123.79000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1700, 10897, 30, 25.89000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1701, 10898, 13, 6.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1702, 10899, 39, 18.00000, 8, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1703, 10900, 70, 15.00000, 3, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1704, 10901, 41, 9.65000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1705, 10901, 71, 21.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1706, 10902, 55, 24.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1707, 10902, 62, 49.30000, 6, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1708, 10903, 13, 6.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1709, 10903, 65, 21.05000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1710, 10903, 68, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1711, 10904, 58, 13.25000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1712, 10904, 62, 49.30000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1713, 10905, 1, 18.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1714, 10906, 61, 28.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1715, 10907, 75, 7.75000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1716, 10908, 7, 30.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1717, 10908, 52, 7.00000, 14, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1718, 10909, 7, 30.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1719, 10909, 16, 17.45000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1720, 10909, 41, 9.65000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1721, 10910, 19, 9.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1722, 10910, 49, 20.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1723, 10910, 61, 28.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1724, 10911, 1, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1725, 10911, 17, 39.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1726, 10911, 67, 14.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1727, 10912, 11, 21.00000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1728, 10912, 29, 123.79000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1729, 10913, 4, 22.00000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1730, 10913, 33, 2.50000, 40, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1731, 10913, 58, 13.25000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1732, 10914, 71, 21.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1733, 10915, 17, 39.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1734, 10915, 33, 2.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1735, 10915, 54, 7.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1736, 10916, 16, 17.45000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1737, 10916, 32, 32.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1738, 10916, 57, 19.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1739, 10917, 30, 25.89000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1740, 10917, 60, 34.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1741, 10918, 1, 18.00000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1742, 10918, 60, 34.00000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1743, 10919, 16, 17.45000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1744, 10919, 25, 14.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1745, 10919, 40, 18.40000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1746, 10920, 50, 16.25000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1747, 10921, 35, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1748, 10921, 63, 43.90000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1749, 10922, 17, 39.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1750, 10922, 24, 4.50000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1751, 10923, 42, 14.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1752, 10923, 43, 46.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1753, 10923, 67, 14.00000, 24, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1754, 10924, 10, 31.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1755, 10924, 28, 45.60000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1756, 10924, 75, 7.75000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1757, 10925, 36, 19.00000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1758, 10925, 52, 7.00000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1759, 10926, 11, 21.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1760, 10926, 13, 6.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1761, 10926, 19, 9.20000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1762, 10926, 72, 34.80000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1763, 10927, 20, 81.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1764, 10927, 52, 7.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1765, 10927, 76, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1766, 10928, 47, 9.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1767, 10928, 76, 18.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1768, 10929, 21, 10.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1769, 10929, 75, 7.75000, 49, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1770, 10929, 77, 13.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1771, 10930, 21, 10.00000, 36, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1772, 10930, 27, 43.90000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1773, 10930, 55, 24.00000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1774, 10930, 58, 13.25000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1775, 10931, 13, 6.00000, 42, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1776, 10931, 57, 19.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1777, 10932, 16, 17.45000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1778, 10932, 62, 49.30000, 14, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1779, 10932, 72, 34.80000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1780, 10932, 75, 7.75000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1781, 10933, 53, 32.80000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1782, 10933, 61, 28.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1783, 10934, 6, 25.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1784, 10935, 1, 18.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1785, 10935, 18, 62.50000, 4, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1786, 10935, 23, 9.00000, 8, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1787, 10936, 36, 19.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1788, 10937, 28, 45.60000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1789, 10937, 34, 14.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1790, 10938, 13, 6.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1791, 10938, 43, 46.00000, 24, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1792, 10938, 60, 34.00000, 49, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1793, 10938, 71, 21.50000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1794, 10939, 2, 19.00000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1795, 10939, 67, 14.00000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1796, 10940, 7, 30.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1797, 10940, 13, 6.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1798, 10941, 31, 12.50000, 44, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1799, 10941, 62, 49.30000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1800, 10941, 68, 12.50000, 80, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1801, 10941, 72, 34.80000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1802, 10942, 49, 20.00000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1803, 10943, 13, 6.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1804, 10943, 22, 21.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1805, 10943, 46, 12.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1806, 10944, 11, 21.00000, 5, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1807, 10944, 44, 19.45000, 18, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1808, 10944, 56, 38.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1809, 10945, 13, 6.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1810, 10945, 31, 12.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1811, 10946, 10, 31.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1812, 10946, 24, 4.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1813, 10946, 77, 13.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1814, 10947, 59, 55.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1815, 10948, 50, 16.25000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1816, 10948, 51, 53.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1817, 10948, 55, 24.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1818, 10949, 6, 25.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1819, 10949, 10, 31.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1820, 10949, 17, 39.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1821, 10949, 62, 49.30000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1822, 10950, 4, 22.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1823, 10951, 33, 2.50000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1824, 10951, 41, 9.65000, 6, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1825, 10951, 75, 7.75000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1826, 10952, 6, 25.00000, 16, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1827, 10952, 28, 45.60000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1828, 10953, 20, 81.00000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1829, 10953, 31, 12.50000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1830, 10954, 16, 17.45000, 28, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1831, 10954, 31, 12.50000, 25, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1832, 10954, 45, 9.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1833, 10954, 60, 34.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1834, 10955, 75, 7.75000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1835, 10956, 21, 10.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1836, 10956, 47, 9.50000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1837, 10956, 51, 53.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1838, 10957, 30, 25.89000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1839, 10957, 35, 18.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1840, 10957, 64, 33.25000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1841, 10958, 5, 21.35000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1842, 10958, 7, 30.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1843, 10958, 72, 34.80000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1844, 10959, 75, 7.75000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1845, 10960, 24, 4.50000, 10, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1846, 10960, 41, 9.65000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1847, 10961, 52, 7.00000, 6, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1848, 10961, 76, 18.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1849, 10962, 7, 30.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1850, 10962, 13, 6.00000, 77, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1851, 10962, 53, 32.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1852, 10962, 69, 36.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1853, 10962, 76, 18.00000, 44, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1854, 10963, 60, 34.00000, 2, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1855, 10964, 18, 62.50000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1856, 10964, 38, 263.50000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1857, 10964, 69, 36.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1858, 10965, 51, 53.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1859, 10966, 37, 26.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1860, 10966, 56, 38.00000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1861, 10966, 62, 49.30000, 12, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1862, 10967, 19, 9.20000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1863, 10967, 49, 20.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1864, 10968, 12, 38.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1865, 10968, 24, 4.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1866, 10968, 64, 33.25000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1867, 10969, 46, 12.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1868, 10970, 52, 7.00000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1869, 10971, 29, 123.79000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1870, 10972, 17, 39.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1871, 10972, 33, 2.50000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1872, 10973, 26, 31.23000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1873, 10973, 41, 9.65000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1874, 10973, 75, 7.75000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1875, 10974, 63, 43.90000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1876, 10975, 8, 40.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1877, 10975, 75, 7.75000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1878, 10976, 28, 45.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1879, 10977, 39, 18.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1880, 10977, 47, 9.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1881, 10977, 51, 53.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1882, 10977, 63, 43.90000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1883, 10978, 8, 40.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1884, 10978, 21, 10.00000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1885, 10978, 40, 18.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1886, 10978, 44, 19.45000, 6, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1887, 10979, 7, 30.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1888, 10979, 12, 38.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1889, 10979, 24, 4.50000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1890, 10979, 27, 43.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1891, 10979, 31, 12.50000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1892, 10979, 63, 43.90000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1893, 10980, 75, 7.75000, 40, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1894, 10981, 38, 263.50000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1895, 10982, 7, 30.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1896, 10982, 43, 46.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1897, 10983, 13, 6.00000, 84, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1898, 10983, 57, 19.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1899, 10984, 16, 17.45000, 55, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1900, 10984, 24, 4.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1901, 10984, 36, 19.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1902, 10985, 16, 17.45000, 36, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1903, 10985, 18, 62.50000, 8, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1904, 10985, 32, 32.00000, 35, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1905, 10986, 11, 21.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1906, 10986, 20, 81.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1907, 10986, 76, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1908, 10986, 77, 13.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1909, 10987, 7, 30.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1910, 10987, 43, 46.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1911, 10987, 72, 34.80000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1912, 10988, 7, 30.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1913, 10988, 62, 49.30000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1914, 10989, 6, 25.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1915, 10989, 11, 21.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1916, 10989, 41, 9.65000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1917, 10990, 21, 10.00000, 65, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1918, 10990, 34, 14.00000, 60, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1919, 10990, 55, 24.00000, 65, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1920, 10990, 61, 28.50000, 66, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1921, 10991, 2, 19.00000, 50, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1922, 10991, 70, 15.00000, 20, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1923, 10991, 76, 18.00000, 90, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1924, 10992, 72, 34.80000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1925, 10993, 29, 123.79000, 50, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1926, 10993, 41, 9.65000, 35, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1927, 10994, 59, 55.00000, 18, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1928, 10995, 51, 53.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1929, 10995, 60, 34.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1930, 10996, 42, 14.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1931, 10997, 32, 32.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1932, 10997, 46, 12.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1933, 10997, 52, 7.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1934, 10998, 24, 4.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1935, 10998, 61, 28.50000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1936, 10998, 74, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1937, 10998, 75, 7.75000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1938, 10999, 41, 9.65000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1939, 10999, 51, 53.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1940, 10999, 77, 13.00000, 21, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1941, 11000, 4, 22.00000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1942, 11000, 24, 4.50000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1943, 11000, 77, 13.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1944, 11001, 7, 30.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1945, 11001, 22, 21.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1946, 11001, 46, 12.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1947, 11001, 55, 24.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1948, 11002, 13, 6.00000, 56, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1949, 11002, 35, 18.00000, 15, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1950, 11002, 42, 14.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1951, 11002, 55, 24.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1952, 11003, 1, 18.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1953, 11003, 40, 18.40000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1954, 11003, 52, 7.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1955, 11004, 26, 31.23000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1956, 11004, 76, 18.00000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1957, 11005, 1, 18.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1958, 11005, 59, 55.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1959, 11006, 1, 18.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1960, 11006, 29, 123.79000, 2, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1961, 11007, 8, 40.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1962, 11007, 29, 123.79000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1963, 11007, 42, 14.00000, 14, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1964, 11008, 28, 45.60000, 70, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1965, 11008, 34, 14.00000, 90, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1966, 11008, 71, 21.50000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1967, 11009, 24, 4.50000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1968, 11009, 36, 19.00000, 18, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1969, 11009, 60, 34.00000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1970, 11010, 7, 30.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1971, 11010, 24, 4.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1972, 11011, 58, 13.25000, 40, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1973, 11011, 71, 21.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1974, 11012, 19, 9.20000, 50, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1975, 11012, 60, 34.00000, 36, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1976, 11012, 71, 21.50000, 60, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1977, 11013, 23, 9.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1978, 11013, 42, 14.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1979, 11013, 45, 9.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1980, 11013, 68, 12.50000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1981, 11014, 41, 9.65000, 28, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1982, 11015, 30, 25.89000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1983, 11015, 77, 13.00000, 18, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1984, 11016, 31, 12.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1985, 11016, 36, 19.00000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1986, 11017, 3, 10.00000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1987, 11017, 59, 55.00000, 110, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1988, 11017, 70, 15.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1989, 11018, 12, 38.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1990, 11018, 18, 62.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1991, 11018, 56, 38.00000, 5, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1992, 11019, 46, 12.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1993, 11019, 49, 20.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1994, 11020, 10, 31.00000, 24, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1995, 11021, 2, 19.00000, 11, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1996, 11021, 20, 81.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1997, 11021, 26, 31.23000, 63, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1998, 11021, 51, 53.00000, 44, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (1999, 11021, 72, 34.80000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2000, 11022, 19, 9.20000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2001, 11022, 69, 36.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2002, 11023, 7, 30.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2003, 11023, 43, 46.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2004, 11024, 26, 31.23000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2005, 11024, 33, 2.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2006, 11024, 65, 21.05000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2007, 11024, 71, 21.50000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2008, 11025, 1, 18.00000, 10, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2009, 11025, 13, 6.00000, 20, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2010, 11026, 18, 62.50000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2011, 11026, 51, 53.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2012, 11027, 24, 4.50000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2013, 11027, 62, 49.30000, 21, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2014, 11028, 55, 24.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2015, 11028, 59, 55.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2016, 11029, 56, 38.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2017, 11029, 63, 43.90000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2018, 11030, 2, 19.00000, 100, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2019, 11030, 5, 21.35000, 70, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2020, 11030, 29, 123.79000, 60, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2021, 11030, 59, 55.00000, 100, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2022, 11031, 1, 18.00000, 45, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2023, 11031, 13, 6.00000, 80, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2024, 11031, 24, 4.50000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2025, 11031, 64, 33.25000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2026, 11031, 71, 21.50000, 16, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2027, 11032, 36, 19.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2028, 11032, 38, 263.50000, 25, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2029, 11032, 59, 55.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2030, 11033, 53, 32.80000, 70, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2031, 11033, 69, 36.00000, 36, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2032, 11034, 21, 10.00000, 15, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2033, 11034, 44, 19.45000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2034, 11034, 61, 28.50000, 6, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2035, 11035, 1, 18.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2036, 11035, 35, 18.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2037, 11035, 42, 14.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2038, 11035, 54, 7.45000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2039, 11036, 13, 6.00000, 7, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2040, 11036, 59, 55.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2041, 11037, 70, 15.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2042, 11038, 40, 18.40000, 5, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2043, 11038, 52, 7.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2044, 11038, 71, 21.50000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2045, 11039, 28, 45.60000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2046, 11039, 35, 18.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2047, 11039, 49, 20.00000, 60, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2048, 11039, 57, 19.50000, 28, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2049, 11040, 21, 10.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2050, 11041, 2, 19.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2051, 11041, 63, 43.90000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2052, 11042, 44, 19.45000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2053, 11042, 61, 28.50000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2054, 11043, 11, 21.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2055, 11044, 62, 49.30000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2056, 11045, 33, 2.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2057, 11045, 51, 53.00000, 24, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2058, 11046, 12, 38.00000, 20, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2059, 11046, 32, 32.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2060, 11046, 35, 18.00000, 18, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2061, 11047, 1, 18.00000, 25, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2062, 11047, 5, 21.35000, 30, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2063, 11048, 68, 12.50000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2064, 11049, 2, 19.00000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2065, 11049, 12, 38.00000, 4, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2066, 11050, 76, 18.00000, 50, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2067, 11051, 24, 4.50000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2068, 11052, 43, 46.00000, 30, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2069, 11052, 61, 28.50000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2070, 11053, 18, 62.50000, 35, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2071, 11053, 32, 32.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2072, 11053, 64, 33.25000, 25, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2073, 11054, 33, 2.50000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2074, 11054, 67, 14.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2075, 11055, 24, 4.50000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2076, 11055, 25, 14.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2077, 11055, 51, 53.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2078, 11055, 57, 19.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2079, 11056, 7, 30.00000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2080, 11056, 55, 24.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2081, 11056, 60, 34.00000, 50, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2082, 11057, 70, 15.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2083, 11058, 21, 10.00000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2084, 11058, 60, 34.00000, 21, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2085, 11058, 61, 28.50000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2086, 11059, 13, 6.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2087, 11059, 17, 39.00000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2088, 11059, 60, 34.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2089, 11060, 60, 34.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2090, 11060, 77, 13.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2091, 11061, 60, 34.00000, 15, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2092, 11062, 53, 32.80000, 10, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2093, 11062, 70, 15.00000, 12, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2094, 11063, 34, 14.00000, 30, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2095, 11063, 40, 18.40000, 40, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2096, 11063, 41, 9.65000, 30, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2097, 11064, 17, 39.00000, 77, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2098, 11064, 41, 9.65000, 12, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2099, 11064, 53, 32.80000, 25, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2100, 11064, 55, 24.00000, 4, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2101, 11064, 68, 12.50000, 55, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2102, 11065, 30, 25.89000, 4, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2103, 11065, 54, 7.45000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2104, 11066, 16, 17.45000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2105, 11066, 19, 9.20000, 42, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2106, 11066, 34, 14.00000, 35, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2107, 11067, 41, 9.65000, 9, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2108, 11068, 28, 45.60000, 8, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2109, 11068, 43, 46.00000, 36, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2110, 11068, 77, 13.00000, 28, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2111, 11069, 39, 18.00000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2112, 11070, 1, 18.00000, 40, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2113, 11070, 2, 19.00000, 20, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2114, 11070, 16, 17.45000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2115, 11070, 31, 12.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2116, 11071, 7, 30.00000, 15, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2117, 11071, 13, 6.00000, 10, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2118, 11072, 2, 19.00000, 8, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2119, 11072, 41, 9.65000, 40, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2120, 11072, 50, 16.25000, 22, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2121, 11072, 64, 33.25000, 130, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2122, 11073, 11, 21.00000, 10, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2123, 11073, 24, 4.50000, 20, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2124, 11074, 16, 17.45000, 14, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2125, 11075, 2, 19.00000, 10, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2126, 11075, 46, 12.00000, 30, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2127, 11075, 76, 18.00000, 2, 0.15000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2128, 11076, 6, 25.00000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2129, 11076, 14, 23.25000, 20, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2130, 11076, 19, 9.20000, 10, 0.25000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2131, 11077, 2, 19.00000, 24, 0.20000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2132, 11077, 3, 10.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2133, 11077, 4, 22.00000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2134, 11077, 6, 25.00000, 1, 0.02000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2135, 11077, 7, 30.00000, 1, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2136, 11077, 8, 40.00000, 2, 0.10000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2137, 11077, 10, 31.00000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2138, 11077, 12, 38.00000, 2, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2139, 11077, 13, 6.00000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2140, 11077, 14, 23.25000, 1, 0.03000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2141, 11077, 16, 17.45000, 2, 0.03000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2142, 11077, 20, 81.00000, 1, 0.04000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2143, 11077, 23, 9.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2144, 11077, 32, 32.00000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2145, 11077, 39, 18.00000, 2, 0.05000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2146, 11077, 41, 9.65000, 3, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2147, 11077, 46, 12.00000, 3, 0.02000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2148, 11077, 52, 7.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2149, 11077, 55, 24.00000, 2, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2150, 11077, 60, 34.00000, 2, 0.06000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2151, 11077, 64, 33.25000, 2, 0.03000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2152, 11077, 66, 17.00000, 1, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2153, 11077, 73, 15.00000, 2, 0.01000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2154, 11077, 75, 7.75000, 4, 0.00000);
INSERT INTO orderlines (orderlineid, orderid, productid, unitprice, quantity, discount) VALUES (2155, 11077, 77, 13.00000, 2, 0.00000);


--
-- TOC entry 1985 (class 0 OID 29494)
-- Dependencies: 1575
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10248, 'VINET', 5, '1996-07-04 00:00:00', '1996-08-01 00:00:00', '1996-07-16 00:00:00', 3, 32.38000, 'Vins et alcools Chevalier', '59 rue de l''Abbaye', 'Reims', 'null', '51100', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10249, 'TOMSP', 6, '1996-07-05 00:00:00', '1996-08-16 00:00:00', '1996-07-10 00:00:00', 1, 11.61000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10250, 'HANAR', 4, '1996-07-08 00:00:00', '1996-08-05 00:00:00', '1996-07-12 00:00:00', 2, 65.83000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10251, 'VICTE', 3, '1996-07-08 00:00:00', '1996-08-05 00:00:00', '1996-07-15 00:00:00', 1, 41.34000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10252, 'SUPRD', 4, '1996-07-09 00:00:00', '1996-08-06 00:00:00', '1996-07-11 00:00:00', 2, 51.30000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10253, 'HANAR', 3, '1996-07-10 00:00:00', '1996-07-24 00:00:00', '1996-07-16 00:00:00', 2, 58.17000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10254, 'CHOPS', 5, '1996-07-11 00:00:00', '1996-08-08 00:00:00', '1996-07-23 00:00:00', 2, 22.98000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10255, 'RICSU', 9, '1996-07-12 00:00:00', '1996-08-09 00:00:00', '1996-07-15 00:00:00', 3, 148.33000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10256, 'WELLI', 3, '1996-07-15 00:00:00', '1996-08-12 00:00:00', '1996-07-17 00:00:00', 2, 13.97000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10257, 'HILAA', 4, '1996-07-16 00:00:00', '1996-08-13 00:00:00', '1996-07-22 00:00:00', 3, 81.91000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10258, 'ERNSH', 1, '1996-07-17 00:00:00', '1996-08-14 00:00:00', '1996-07-23 00:00:00', 1, 140.51000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10259, 'CENTC', 4, '1996-07-18 00:00:00', '1996-08-15 00:00:00', '1996-07-25 00:00:00', 3, 3.25000, 'Centro comercial Moctezuma', 'Sierras de Granada 9993', 'México D.F.', 'null', '05022', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10260, 'OTTIK', 4, '1996-07-19 00:00:00', '1996-08-16 00:00:00', '1996-07-29 00:00:00', 1, 55.09000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10261, 'QUEDE', 4, '1996-07-19 00:00:00', '1996-08-16 00:00:00', '1996-07-30 00:00:00', 2, 3.05000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10262, 'RATTC', 8, '1996-07-22 00:00:00', '1996-08-19 00:00:00', '1996-07-25 00:00:00', 3, 48.29000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10263, 'ERNSH', 9, '1996-07-23 00:00:00', '1996-08-20 00:00:00', '1996-07-31 00:00:00', 3, 146.06000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10264, 'FOLKO', 6, '1996-07-24 00:00:00', '1996-08-21 00:00:00', '1996-08-23 00:00:00', 3, 3.67000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10265, 'BLONP', 2, '1996-07-25 00:00:00', '1996-08-22 00:00:00', '1996-08-12 00:00:00', 1, 55.28000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10266, 'WARTH', 3, '1996-07-26 00:00:00', '1996-09-06 00:00:00', '1996-07-31 00:00:00', 3, 25.73000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10267, 'FRANK', 4, '1996-07-29 00:00:00', '1996-08-26 00:00:00', '1996-08-06 00:00:00', 1, 208.58000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10268, 'GROSR', 8, '1996-07-30 00:00:00', '1996-08-27 00:00:00', '1996-08-02 00:00:00', 3, 66.29000, 'GROSELLA-Restaurante', '5ª Ave. Los Palos Grandes', 'Caracas', 'DF', '1081', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10269, 'WHITC', 5, '1996-07-31 00:00:00', '1996-08-14 00:00:00', '1996-08-09 00:00:00', 1, 4.56000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10270, 'WARTH', 1, '1996-08-01 00:00:00', '1996-08-29 00:00:00', '1996-08-02 00:00:00', 1, 136.54000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10271, 'SPLIR', 6, '1996-08-01 00:00:00', '1996-08-29 00:00:00', '1996-08-30 00:00:00', 2, 4.54000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10272, 'RATTC', 6, '1996-08-02 00:00:00', '1996-08-30 00:00:00', '1996-08-06 00:00:00', 2, 98.03000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10273, 'QUICK', 3, '1996-08-05 00:00:00', '1996-09-02 00:00:00', '1996-08-12 00:00:00', 3, 76.07000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10274, 'VINET', 6, '1996-08-06 00:00:00', '1996-09-03 00:00:00', '1996-08-16 00:00:00', 1, 6.01000, 'Vins et alcools Chevalier', '59 rue de l''Abbaye', 'Reims', 'null', '51100', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10275, 'MAGAA', 1, '1996-08-07 00:00:00', '1996-09-04 00:00:00', '1996-08-09 00:00:00', 1, 26.93000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10276, 'TORTU', 8, '1996-08-08 00:00:00', '1996-08-22 00:00:00', '1996-08-14 00:00:00', 3, 13.84000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10277, 'MORGK', 2, '1996-08-09 00:00:00', '1996-09-06 00:00:00', '1996-08-13 00:00:00', 3, 125.77000, 'Morgenstern Gesundkost', 'Heerstr. 22', 'Leipzig', 'null', '04179', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10278, 'BERGS', 8, '1996-08-12 00:00:00', '1996-09-09 00:00:00', '1996-08-16 00:00:00', 2, 92.69000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10279, 'LEHMS', 8, '1996-08-13 00:00:00', '1996-09-10 00:00:00', '1996-08-16 00:00:00', 2, 25.83000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10280, 'BERGS', 2, '1996-08-14 00:00:00', '1996-09-11 00:00:00', '1996-09-12 00:00:00', 1, 8.98000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10281, 'ROMEY', 4, '1996-08-14 00:00:00', '1996-08-28 00:00:00', '1996-08-21 00:00:00', 1, 2.94000, 'Romero y tomillo', 'Gran Vía, 1', 'Madrid', 'null', '28001', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10282, 'ROMEY', 4, '1996-08-15 00:00:00', '1996-09-12 00:00:00', '1996-08-21 00:00:00', 1, 12.69000, 'Romero y tomillo', 'Gran Vía, 1', 'Madrid', 'null', '28001', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10283, 'LILAS', 3, '1996-08-16 00:00:00', '1996-09-13 00:00:00', '1996-08-23 00:00:00', 3, 84.81000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10284, 'LEHMS', 4, '1996-08-19 00:00:00', '1996-09-16 00:00:00', '1996-08-27 00:00:00', 1, 76.56000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10285, 'QUICK', 1, '1996-08-20 00:00:00', '1996-09-17 00:00:00', '1996-08-26 00:00:00', 2, 76.83000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10286, 'QUICK', 8, '1996-08-21 00:00:00', '1996-09-18 00:00:00', '1996-08-30 00:00:00', 3, 229.24000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10287, 'RICAR', 8, '1996-08-22 00:00:00', '1996-09-19 00:00:00', '1996-08-28 00:00:00', 3, 12.76000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10288, 'REGGC', 4, '1996-08-23 00:00:00', '1996-09-20 00:00:00', '1996-09-03 00:00:00', 1, 7.45000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10289, 'BSBEV', 7, '1996-08-26 00:00:00', '1996-09-23 00:00:00', '1996-08-28 00:00:00', 3, 22.77000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10290, 'COMMI', 8, '1996-08-27 00:00:00', '1996-09-24 00:00:00', '1996-09-03 00:00:00', 1, 79.70000, 'Comércio Mineiro', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10291, 'QUEDE', 6, '1996-08-27 00:00:00', '1996-09-24 00:00:00', '1996-09-04 00:00:00', 2, 6.40000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10292, 'TRADH', 1, '1996-08-28 00:00:00', '1996-09-25 00:00:00', '1996-09-02 00:00:00', 2, 1.35000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10293, 'TORTU', 1, '1996-08-29 00:00:00', '1996-09-26 00:00:00', '1996-09-11 00:00:00', 3, 21.18000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10294, 'RATTC', 4, '1996-08-30 00:00:00', '1996-09-27 00:00:00', '1996-09-05 00:00:00', 2, 147.26000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10295, 'VINET', 2, '1996-09-02 00:00:00', '1996-09-30 00:00:00', '1996-09-10 00:00:00', 2, 1.15000, 'Vins et alcools Chevalier', '59 rue de l''Abbaye', 'Reims', 'null', '51100', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10296, 'LILAS', 6, '1996-09-03 00:00:00', '1996-10-01 00:00:00', '1996-09-11 00:00:00', 1, 0.12000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10297, 'BLONP', 5, '1996-09-04 00:00:00', '1996-10-16 00:00:00', '1996-09-10 00:00:00', 2, 5.74000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10298, 'HUNGO', 6, '1996-09-05 00:00:00', '1996-10-03 00:00:00', '1996-09-11 00:00:00', 2, 168.22000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10299, 'RICAR', 4, '1996-09-06 00:00:00', '1996-10-04 00:00:00', '1996-09-13 00:00:00', 2, 29.76000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10300, 'MAGAA', 2, '1996-09-09 00:00:00', '1996-10-07 00:00:00', '1996-09-18 00:00:00', 2, 17.68000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10301, 'WANDK', 8, '1996-09-09 00:00:00', '1996-10-07 00:00:00', '1996-09-17 00:00:00', 2, 45.08000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10302, 'SUPRD', 4, '1996-09-10 00:00:00', '1996-10-08 00:00:00', '1996-10-09 00:00:00', 2, 6.27000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10303, 'GODOS', 7, '1996-09-11 00:00:00', '1996-10-09 00:00:00', '1996-09-18 00:00:00', 2, 107.83000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10304, 'TORTU', 1, '1996-09-12 00:00:00', '1996-10-10 00:00:00', '1996-09-17 00:00:00', 2, 63.79000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10305, 'OLDWO', 8, '1996-09-13 00:00:00', '1996-10-11 00:00:00', '1996-10-09 00:00:00', 3, 257.62000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10306, 'ROMEY', 1, '1996-09-16 00:00:00', '1996-10-14 00:00:00', '1996-09-23 00:00:00', 3, 7.56000, 'Romero y tomillo', 'Gran Vía, 1', 'Madrid', 'null', '28001', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10307, 'LONEP', 2, '1996-09-17 00:00:00', '1996-10-15 00:00:00', '1996-09-25 00:00:00', 2, 0.56000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10308, 'ANATR', 7, '1996-09-18 00:00:00', '1996-10-16 00:00:00', '1996-09-24 00:00:00', 3, 1.61000, 'Ana Trujillo Emparedados y helados', 'Avda. de la Constitución 2222', 'México D.F.', 'null', '05021', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10309, 'HUNGO', 3, '1996-09-19 00:00:00', '1996-10-17 00:00:00', '1996-10-23 00:00:00', 1, 47.30000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10310, 'THEBI', 8, '1996-09-20 00:00:00', '1996-10-18 00:00:00', '1996-09-27 00:00:00', 2, 17.52000, 'The Big Cheese', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10311, 'DUMON', 1, '1996-09-20 00:00:00', '1996-10-04 00:00:00', '1996-09-26 00:00:00', 3, 24.69000, 'Du monde entier', '67, rue des Cinquante Otages', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10312, 'WANDK', 2, '1996-09-23 00:00:00', '1996-10-21 00:00:00', '1996-10-03 00:00:00', 2, 40.26000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10313, 'QUICK', 2, '1996-09-24 00:00:00', '1996-10-22 00:00:00', '1996-10-04 00:00:00', 2, 1.96000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10314, 'RATTC', 1, '1996-09-25 00:00:00', '1996-10-23 00:00:00', '1996-10-04 00:00:00', 2, 74.16000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10315, 'ISLAT', 4, '1996-09-26 00:00:00', '1996-10-24 00:00:00', '1996-10-03 00:00:00', 2, 41.76000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10316, 'RATTC', 1, '1996-09-27 00:00:00', '1996-10-25 00:00:00', '1996-10-08 00:00:00', 3, 150.15000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10317, 'LONEP', 6, '1996-09-30 00:00:00', '1996-10-28 00:00:00', '1996-10-10 00:00:00', 1, 12.69000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10318, 'ISLAT', 8, '1996-10-01 00:00:00', '1996-10-29 00:00:00', '1996-10-04 00:00:00', 2, 4.73000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10319, 'TORTU', 7, '1996-10-02 00:00:00', '1996-10-30 00:00:00', '1996-10-11 00:00:00', 3, 64.50000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10320, 'WARTH', 5, '1996-10-03 00:00:00', '1996-10-17 00:00:00', '1996-10-18 00:00:00', 3, 34.57000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10321, 'ISLAT', 3, '1996-10-03 00:00:00', '1996-10-31 00:00:00', '1996-10-11 00:00:00', 2, 3.43000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10322, 'PERIC', 7, '1996-10-04 00:00:00', '1996-11-01 00:00:00', '1996-10-23 00:00:00', 3, 0.40000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10323, 'KOENE', 4, '1996-10-07 00:00:00', '1996-11-04 00:00:00', '1996-10-14 00:00:00', 1, 4.88000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10324, 'SAVEA', 9, '1996-10-08 00:00:00', '1996-11-05 00:00:00', '1996-10-10 00:00:00', 1, 214.27000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10325, 'KOENE', 1, '1996-10-09 00:00:00', '1996-10-23 00:00:00', '1996-10-14 00:00:00', 3, 64.86000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10326, 'BOLID', 4, '1996-10-10 00:00:00', '1996-11-07 00:00:00', '1996-10-14 00:00:00', 2, 77.92000, 'Bólido Comidas preparadas', 'C/ Araquil, 67', 'Madrid', 'null', '28023', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10327, 'FOLKO', 2, '1996-10-11 00:00:00', '1996-11-08 00:00:00', '1996-10-14 00:00:00', 1, 63.36000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10328, 'FURIB', 4, '1996-10-14 00:00:00', '1996-11-11 00:00:00', '1996-10-17 00:00:00', 3, 87.03000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10329, 'SPLIR', 4, '1996-10-15 00:00:00', '1996-11-26 00:00:00', '1996-10-23 00:00:00', 2, 191.67000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10330, 'LILAS', 3, '1996-10-16 00:00:00', '1996-11-13 00:00:00', '1996-10-28 00:00:00', 1, 12.75000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10331, 'BONAP', 9, '1996-10-16 00:00:00', '1996-11-27 00:00:00', '1996-10-21 00:00:00', 1, 10.19000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10332, 'MEREP', 3, '1996-10-17 00:00:00', '1996-11-28 00:00:00', '1996-10-21 00:00:00', 2, 52.84000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10333, 'WARTH', 5, '1996-10-18 00:00:00', '1996-11-15 00:00:00', '1996-10-25 00:00:00', 3, 0.59000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10334, 'VICTE', 8, '1996-10-21 00:00:00', '1996-11-18 00:00:00', '1996-10-28 00:00:00', 2, 8.56000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10335, 'HUNGO', 7, '1996-10-22 00:00:00', '1996-11-19 00:00:00', '1996-10-24 00:00:00', 2, 42.11000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10336, 'PRINI', 7, '1996-10-23 00:00:00', '1996-11-20 00:00:00', '1996-10-25 00:00:00', 2, 15.51000, 'Princesa Isabel Vinhos', 'Estrada da saúde n. 58', 'Lisboa', 'null', '1756', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10337, 'FRANK', 4, '1996-10-24 00:00:00', '1996-11-21 00:00:00', '1996-10-29 00:00:00', 3, 108.26000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10338, 'OLDWO', 4, '1996-10-25 00:00:00', '1996-11-22 00:00:00', '1996-10-29 00:00:00', 3, 84.21000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10339, 'MEREP', 2, '1996-10-28 00:00:00', '1996-11-25 00:00:00', '1996-11-04 00:00:00', 2, 15.66000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10340, 'BONAP', 1, '1996-10-29 00:00:00', '1996-11-26 00:00:00', '1996-11-08 00:00:00', 3, 166.31000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10341, 'SIMOB', 7, '1996-10-29 00:00:00', '1996-11-26 00:00:00', '1996-11-05 00:00:00', 3, 26.78000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10342, 'FRANK', 4, '1996-10-30 00:00:00', '1996-11-13 00:00:00', '1996-11-04 00:00:00', 2, 54.83000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10343, 'LEHMS', 4, '1996-10-31 00:00:00', '1996-11-28 00:00:00', '1996-11-06 00:00:00', 1, 110.37000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10344, 'WHITC', 4, '1996-11-01 00:00:00', '1996-11-29 00:00:00', '1996-11-05 00:00:00', 2, 23.29000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10345, 'QUICK', 2, '1996-11-04 00:00:00', '1996-12-02 00:00:00', '1996-11-11 00:00:00', 2, 249.06000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10346, 'RATTC', 3, '1996-11-05 00:00:00', '1996-12-17 00:00:00', '1996-11-08 00:00:00', 3, 142.08000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10347, 'FAMIA', 4, '1996-11-06 00:00:00', '1996-12-04 00:00:00', '1996-11-08 00:00:00', 3, 3.10000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10348, 'WANDK', 4, '1996-11-07 00:00:00', '1996-12-05 00:00:00', '1996-11-15 00:00:00', 2, 0.78000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10349, 'SPLIR', 7, '1996-11-08 00:00:00', '1996-12-06 00:00:00', '1996-11-15 00:00:00', 1, 8.63000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10350, 'LAMAI', 6, '1996-11-11 00:00:00', '1996-12-09 00:00:00', '1996-12-03 00:00:00', 2, 64.19000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10351, 'ERNSH', 1, '1996-11-11 00:00:00', '1996-12-09 00:00:00', '1996-11-20 00:00:00', 1, 162.33000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10352, 'FURIB', 3, '1996-11-12 00:00:00', '1996-11-26 00:00:00', '1996-11-18 00:00:00', 3, 1.30000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10353, 'PICCO', 7, '1996-11-13 00:00:00', '1996-12-11 00:00:00', '1996-11-25 00:00:00', 3, 360.63000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10354, 'PERIC', 8, '1996-11-14 00:00:00', '1996-12-12 00:00:00', '1996-11-20 00:00:00', 3, 53.80000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10355, 'AROUT', 6, '1996-11-15 00:00:00', '1996-12-13 00:00:00', '1996-11-20 00:00:00', 1, 41.95000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10356, 'WANDK', 6, '1996-11-18 00:00:00', '1996-12-16 00:00:00', '1996-11-27 00:00:00', 2, 36.71000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10357, 'LILAS', 1, '1996-11-19 00:00:00', '1996-12-17 00:00:00', '1996-12-02 00:00:00', 3, 34.88000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10358, 'LAMAI', 5, '1996-11-20 00:00:00', '1996-12-18 00:00:00', '1996-11-27 00:00:00', 1, 19.64000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10359, 'SEVES', 5, '1996-11-21 00:00:00', '1996-12-19 00:00:00', '1996-11-26 00:00:00', 3, 288.43000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10360, 'BLONP', 4, '1996-11-22 00:00:00', '1996-12-20 00:00:00', '1996-12-02 00:00:00', 3, 131.70000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10361, 'QUICK', 1, '1996-11-22 00:00:00', '1996-12-20 00:00:00', '1996-12-03 00:00:00', 2, 183.17000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10362, 'BONAP', 3, '1996-11-25 00:00:00', '1996-12-23 00:00:00', '1996-11-28 00:00:00', 1, 96.04000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10363, 'DRACD', 4, '1996-11-26 00:00:00', '1996-12-24 00:00:00', '1996-12-04 00:00:00', 3, 30.54000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10364, 'EASTC', 1, '1996-11-26 00:00:00', '1997-01-07 00:00:00', '1996-12-04 00:00:00', 1, 71.97000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10365, 'ANTON', 3, '1996-11-27 00:00:00', '1996-12-25 00:00:00', '1996-12-02 00:00:00', 2, 22.00000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10366, 'GALED', 8, '1996-11-28 00:00:00', '1997-01-09 00:00:00', '1996-12-30 00:00:00', 2, 10.14000, 'Galería del gastronómo', 'Rambla de Cataluña, 23', 'Barcelona', 'null', '8022', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10367, 'VAFFE', 7, '1996-11-28 00:00:00', '1996-12-26 00:00:00', '1996-12-02 00:00:00', 3, 13.55000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10368, 'ERNSH', 2, '1996-11-29 00:00:00', '1996-12-27 00:00:00', '1996-12-02 00:00:00', 2, 101.95000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10369, 'SPLIR', 8, '1996-12-02 00:00:00', '1996-12-30 00:00:00', '1996-12-09 00:00:00', 2, 195.68000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10370, 'CHOPS', 6, '1996-12-03 00:00:00', '1996-12-31 00:00:00', '1996-12-27 00:00:00', 2, 1.17000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10371, 'LAMAI', 1, '1996-12-03 00:00:00', '1996-12-31 00:00:00', '1996-12-24 00:00:00', 1, 0.45000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10372, 'QUEEN', 5, '1996-12-04 00:00:00', '1997-01-01 00:00:00', '1996-12-09 00:00:00', 2, 890.78000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10373, 'HUNGO', 4, '1996-12-05 00:00:00', '1997-01-02 00:00:00', '1996-12-11 00:00:00', 3, 124.12000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10374, 'WOLZA', 1, '1996-12-05 00:00:00', '1997-01-02 00:00:00', '1996-12-09 00:00:00', 3, 3.94000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10375, 'HUNGC', 3, '1996-12-06 00:00:00', '1997-01-03 00:00:00', '1996-12-09 00:00:00', 2, 20.12000, 'Hungry Coyote Import Store', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10376, 'MEREP', 1, '1996-12-09 00:00:00', '1997-01-06 00:00:00', '1996-12-13 00:00:00', 2, 20.39000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10377, 'SEVES', 1, '1996-12-09 00:00:00', '1997-01-06 00:00:00', '1996-12-13 00:00:00', 3, 22.21000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10378, 'FOLKO', 5, '1996-12-10 00:00:00', '1997-01-07 00:00:00', '1996-12-19 00:00:00', 3, 5.44000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10379, 'QUEDE', 2, '1996-12-11 00:00:00', '1997-01-08 00:00:00', '1996-12-13 00:00:00', 1, 45.03000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10380, 'HUNGO', 8, '1996-12-12 00:00:00', '1997-01-09 00:00:00', '1997-01-16 00:00:00', 3, 35.03000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10381, 'LILAS', 3, '1996-12-12 00:00:00', '1997-01-09 00:00:00', '1996-12-13 00:00:00', 3, 7.99000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10382, 'ERNSH', 4, '1996-12-13 00:00:00', '1997-01-10 00:00:00', '1996-12-16 00:00:00', 1, 94.77000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10383, 'AROUT', 8, '1996-12-16 00:00:00', '1997-01-13 00:00:00', '1996-12-18 00:00:00', 3, 34.24000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10384, 'BERGS', 3, '1996-12-16 00:00:00', '1997-01-13 00:00:00', '1996-12-20 00:00:00', 3, 168.64000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10385, 'SPLIR', 1, '1996-12-17 00:00:00', '1997-01-14 00:00:00', '1996-12-23 00:00:00', 2, 30.96000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10386, 'FAMIA', 9, '1996-12-18 00:00:00', '1997-01-01 00:00:00', '1996-12-25 00:00:00', 3, 13.99000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10387, 'SANTG', 1, '1996-12-18 00:00:00', '1997-01-15 00:00:00', '1996-12-20 00:00:00', 2, 93.63000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10388, 'SEVES', 2, '1996-12-19 00:00:00', '1997-01-16 00:00:00', '1996-12-20 00:00:00', 1, 34.86000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10389, 'BOTTM', 4, '1996-12-20 00:00:00', '1997-01-17 00:00:00', '1996-12-24 00:00:00', 2, 47.42000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10390, 'ERNSH', 6, '1996-12-23 00:00:00', '1997-01-20 00:00:00', '1996-12-26 00:00:00', 1, 126.38000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10391, 'DRACD', 3, '1996-12-23 00:00:00', '1997-01-20 00:00:00', '1996-12-31 00:00:00', 3, 5.45000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10392, 'PICCO', 2, '1996-12-24 00:00:00', '1997-01-21 00:00:00', '1997-01-01 00:00:00', 3, 122.46000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10393, 'SAVEA', 1, '1996-12-25 00:00:00', '1997-01-22 00:00:00', '1997-01-03 00:00:00', 3, 126.56000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10394, 'HUNGC', 1, '1996-12-25 00:00:00', '1997-01-22 00:00:00', '1997-01-03 00:00:00', 3, 30.34000, 'Hungry Coyote Import Store', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10395, 'HILAA', 6, '1996-12-26 00:00:00', '1997-01-23 00:00:00', '1997-01-03 00:00:00', 1, 184.41000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10396, 'FRANK', 1, '1996-12-27 00:00:00', '1997-01-10 00:00:00', '1997-01-06 00:00:00', 3, 135.35000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10397, 'PRINI', 5, '1996-12-27 00:00:00', '1997-01-24 00:00:00', '1997-01-02 00:00:00', 1, 60.26000, 'Princesa Isabel Vinhos', 'Estrada da saúde n. 58', 'Lisboa', 'null', '1756', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10398, 'SAVEA', 2, '1996-12-30 00:00:00', '1997-01-27 00:00:00', '1997-01-09 00:00:00', 3, 89.16000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10399, 'VAFFE', 8, '1996-12-31 00:00:00', '1997-01-14 00:00:00', '1997-01-08 00:00:00', 3, 27.36000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10400, 'EASTC', 1, '1997-01-01 00:00:00', '1997-01-29 00:00:00', '1997-01-16 00:00:00', 3, 83.93000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10401, 'RATTC', 1, '1997-01-01 00:00:00', '1997-01-29 00:00:00', '1997-01-10 00:00:00', 1, 12.51000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10402, 'ERNSH', 8, '1997-01-02 00:00:00', '1997-02-13 00:00:00', '1997-01-10 00:00:00', 2, 67.88000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10403, 'ERNSH', 4, '1997-01-03 00:00:00', '1997-01-31 00:00:00', '1997-01-09 00:00:00', 3, 73.79000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10404, 'MAGAA', 2, '1997-01-03 00:00:00', '1997-01-31 00:00:00', '1997-01-08 00:00:00', 1, 155.97000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10405, 'LINOD', 1, '1997-01-06 00:00:00', '1997-02-03 00:00:00', '1997-01-22 00:00:00', 1, 34.82000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10406, 'QUEEN', 7, '1997-01-07 00:00:00', '1997-02-18 00:00:00', '1997-01-13 00:00:00', 1, 108.04000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10407, 'OTTIK', 2, '1997-01-07 00:00:00', '1997-02-04 00:00:00', '1997-01-30 00:00:00', 2, 91.48000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10408, 'FOLIG', 8, '1997-01-08 00:00:00', '1997-02-05 00:00:00', '1997-01-14 00:00:00', 1, 11.26000, 'Folies gourmandes', '184, chaussée de Tournai', 'Lille', 'null', '59000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10409, 'OCEAN', 3, '1997-01-09 00:00:00', '1997-02-06 00:00:00', '1997-01-14 00:00:00', 1, 29.83000, 'Océano Atlántico Ltda.', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10410, 'BOTTM', 3, '1997-01-10 00:00:00', '1997-02-07 00:00:00', '1997-01-15 00:00:00', 3, 2.40000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10411, 'BOTTM', 9, '1997-01-10 00:00:00', '1997-02-07 00:00:00', '1997-01-21 00:00:00', 3, 23.65000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10412, 'WARTH', 8, '1997-01-13 00:00:00', '1997-02-10 00:00:00', '1997-01-15 00:00:00', 2, 3.77000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10413, 'LAMAI', 3, '1997-01-14 00:00:00', '1997-02-11 00:00:00', '1997-01-16 00:00:00', 2, 95.66000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10414, 'FAMIA', 2, '1997-01-14 00:00:00', '1997-02-11 00:00:00', '1997-01-17 00:00:00', 3, 21.48000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10415, 'HUNGC', 3, '1997-01-15 00:00:00', '1997-02-12 00:00:00', '1997-01-24 00:00:00', 1, 0.20000, 'Hungry Coyote Import Store', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10416, 'WARTH', 8, '1997-01-16 00:00:00', '1997-02-13 00:00:00', '1997-01-27 00:00:00', 3, 22.72000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10417, 'SIMOB', 4, '1997-01-16 00:00:00', '1997-02-13 00:00:00', '1997-01-28 00:00:00', 3, 70.29000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10418, 'QUICK', 4, '1997-01-17 00:00:00', '1997-02-14 00:00:00', '1997-01-24 00:00:00', 1, 17.55000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10419, 'RICSU', 4, '1997-01-20 00:00:00', '1997-02-17 00:00:00', '1997-01-30 00:00:00', 2, 137.35000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10420, 'WELLI', 3, '1997-01-21 00:00:00', '1997-02-18 00:00:00', '1997-01-27 00:00:00', 1, 44.12000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10421, 'QUEDE', 8, '1997-01-21 00:00:00', '1997-03-04 00:00:00', '1997-01-27 00:00:00', 1, 99.23000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10422, 'FRANS', 2, '1997-01-22 00:00:00', '1997-02-19 00:00:00', '1997-01-31 00:00:00', 1, 3.02000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10423, 'GOURL', 6, '1997-01-23 00:00:00', '1997-02-06 00:00:00', '1997-02-24 00:00:00', 3, 24.50000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10424, 'MEREP', 7, '1997-01-23 00:00:00', '1997-02-20 00:00:00', '1997-01-27 00:00:00', 2, 370.61000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10425, 'LAMAI', 6, '1997-01-24 00:00:00', '1997-02-21 00:00:00', '1997-02-14 00:00:00', 2, 7.93000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10426, 'GALED', 4, '1997-01-27 00:00:00', '1997-02-24 00:00:00', '1997-02-06 00:00:00', 1, 18.69000, 'Galería del gastronómo', 'Rambla de Cataluña, 23', 'Barcelona', 'null', '8022', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10427, 'PICCO', 4, '1997-01-27 00:00:00', '1997-02-24 00:00:00', '1997-03-03 00:00:00', 2, 31.29000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10428, 'REGGC', 7, '1997-01-28 00:00:00', '1997-02-25 00:00:00', '1997-02-04 00:00:00', 1, 11.09000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10429, 'HUNGO', 3, '1997-01-29 00:00:00', '1997-03-12 00:00:00', '1997-02-07 00:00:00', 2, 56.63000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10430, 'ERNSH', 4, '1997-01-30 00:00:00', '1997-02-13 00:00:00', '1997-02-03 00:00:00', 1, 458.78000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10431, 'BOTTM', 4, '1997-01-30 00:00:00', '1997-02-13 00:00:00', '1997-02-07 00:00:00', 2, 44.17000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10432, 'SPLIR', 3, '1997-01-31 00:00:00', '1997-02-14 00:00:00', '1997-02-07 00:00:00', 2, 4.34000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10433, 'PRINI', 3, '1997-02-03 00:00:00', '1997-03-03 00:00:00', '1997-03-04 00:00:00', 3, 73.83000, 'Princesa Isabel Vinhos', 'Estrada da saúde n. 58', 'Lisboa', 'null', '1756', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10434, 'FOLKO', 3, '1997-02-03 00:00:00', '1997-03-03 00:00:00', '1997-02-13 00:00:00', 2, 17.92000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10435, 'CONSH', 8, '1997-02-04 00:00:00', '1997-03-18 00:00:00', '1997-02-07 00:00:00', 2, 9.21000, 'Consolidated Holdings', 'Berkeley Gardens 12  Brewery', 'London', 'null', 'WX1 6LT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10436, 'BLONP', 3, '1997-02-05 00:00:00', '1997-03-05 00:00:00', '1997-02-11 00:00:00', 2, 156.66000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10437, 'WARTH', 8, '1997-02-05 00:00:00', '1997-03-05 00:00:00', '1997-02-12 00:00:00', 1, 19.97000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10438, 'TOMSP', 3, '1997-02-06 00:00:00', '1997-03-06 00:00:00', '1997-02-14 00:00:00', 2, 8.24000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10439, 'MEREP', 6, '1997-02-07 00:00:00', '1997-03-07 00:00:00', '1997-02-10 00:00:00', 3, 4.07000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10440, 'SAVEA', 4, '1997-02-10 00:00:00', '1997-03-10 00:00:00', '1997-02-28 00:00:00', 2, 86.53000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10441, 'OLDWO', 3, '1997-02-10 00:00:00', '1997-03-24 00:00:00', '1997-03-14 00:00:00', 2, 73.02000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10442, 'ERNSH', 3, '1997-02-11 00:00:00', '1997-03-11 00:00:00', '1997-02-18 00:00:00', 2, 47.94000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10443, 'REGGC', 8, '1997-02-12 00:00:00', '1997-03-12 00:00:00', '1997-02-14 00:00:00', 1, 13.95000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10444, 'BERGS', 3, '1997-02-12 00:00:00', '1997-03-12 00:00:00', '1997-02-21 00:00:00', 3, 3.50000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10445, 'BERGS', 3, '1997-02-13 00:00:00', '1997-03-13 00:00:00', '1997-02-20 00:00:00', 1, 9.30000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10446, 'TOMSP', 6, '1997-02-14 00:00:00', '1997-03-14 00:00:00', '1997-02-19 00:00:00', 1, 14.68000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10447, 'RICAR', 4, '1997-02-14 00:00:00', '1997-03-14 00:00:00', '1997-03-07 00:00:00', 2, 68.66000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10448, 'RANCH', 4, '1997-02-17 00:00:00', '1997-03-17 00:00:00', '1997-02-24 00:00:00', 2, 38.82000, 'Rancho grande', 'Av. del Libertador 900', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10449, 'BLONP', 3, '1997-02-18 00:00:00', '1997-03-18 00:00:00', '1997-02-27 00:00:00', 2, 53.30000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10450, 'VICTE', 8, '1997-02-19 00:00:00', '1997-03-19 00:00:00', '1997-03-11 00:00:00', 2, 7.23000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10451, 'QUICK', 4, '1997-02-19 00:00:00', '1997-03-05 00:00:00', '1997-03-12 00:00:00', 3, 189.09000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10452, 'SAVEA', 8, '1997-02-20 00:00:00', '1997-03-20 00:00:00', '1997-02-26 00:00:00', 1, 140.26000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10453, 'AROUT', 1, '1997-02-21 00:00:00', '1997-03-21 00:00:00', '1997-02-26 00:00:00', 2, 25.36000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10454, 'LAMAI', 4, '1997-02-21 00:00:00', '1997-03-21 00:00:00', '1997-02-25 00:00:00', 3, 2.74000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10455, 'WARTH', 8, '1997-02-24 00:00:00', '1997-04-07 00:00:00', '1997-03-03 00:00:00', 2, 180.45000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10456, 'KOENE', 8, '1997-02-25 00:00:00', '1997-04-08 00:00:00', '1997-02-28 00:00:00', 2, 8.12000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10457, 'KOENE', 2, '1997-02-25 00:00:00', '1997-03-25 00:00:00', '1997-03-03 00:00:00', 1, 11.57000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10458, 'SUPRD', 7, '1997-02-26 00:00:00', '1997-03-26 00:00:00', '1997-03-04 00:00:00', 3, 147.06000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10459, 'VICTE', 4, '1997-02-27 00:00:00', '1997-03-27 00:00:00', '1997-02-28 00:00:00', 2, 25.09000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10460, 'FOLKO', 8, '1997-02-28 00:00:00', '1997-03-28 00:00:00', '1997-03-03 00:00:00', 1, 16.27000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10461, 'LILAS', 1, '1997-02-28 00:00:00', '1997-03-28 00:00:00', '1997-03-05 00:00:00', 3, 148.61000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10462, 'CONSH', 2, '1997-03-03 00:00:00', '1997-03-31 00:00:00', '1997-03-18 00:00:00', 1, 6.17000, 'Consolidated Holdings', 'Berkeley Gardens 12  Brewery', 'London', 'null', 'WX1 6LT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10463, 'SUPRD', 5, '1997-03-04 00:00:00', '1997-04-01 00:00:00', '1997-03-06 00:00:00', 3, 14.78000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10464, 'FURIB', 4, '1997-03-04 00:00:00', '1997-04-01 00:00:00', '1997-03-14 00:00:00', 2, 89.00000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10465, 'VAFFE', 1, '1997-03-05 00:00:00', '1997-04-02 00:00:00', '1997-03-14 00:00:00', 3, 145.04000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10466, 'COMMI', 4, '1997-03-06 00:00:00', '1997-04-03 00:00:00', '1997-03-13 00:00:00', 1, 11.93000, 'Comércio Mineiro', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10467, 'MAGAA', 8, '1997-03-06 00:00:00', '1997-04-03 00:00:00', '1997-03-11 00:00:00', 2, 4.93000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10468, 'KOENE', 3, '1997-03-07 00:00:00', '1997-04-04 00:00:00', '1997-03-12 00:00:00', 3, 44.12000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10469, 'WHITC', 1, '1997-03-10 00:00:00', '1997-04-07 00:00:00', '1997-03-14 00:00:00', 1, 60.18000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10470, 'BONAP', 4, '1997-03-11 00:00:00', '1997-04-08 00:00:00', '1997-03-14 00:00:00', 2, 64.56000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10471, 'BSBEV', 2, '1997-03-11 00:00:00', '1997-04-08 00:00:00', '1997-03-18 00:00:00', 3, 45.59000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10472, 'SEVES', 8, '1997-03-12 00:00:00', '1997-04-09 00:00:00', '1997-03-19 00:00:00', 1, 4.20000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10473, 'ISLAT', 1, '1997-03-13 00:00:00', '1997-03-27 00:00:00', '1997-03-21 00:00:00', 3, 16.37000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10474, 'PERIC', 5, '1997-03-13 00:00:00', '1997-04-10 00:00:00', '1997-03-21 00:00:00', 2, 83.49000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10475, 'SUPRD', 9, '1997-03-14 00:00:00', '1997-04-11 00:00:00', '1997-04-04 00:00:00', 1, 68.52000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10476, 'HILAA', 8, '1997-03-17 00:00:00', '1997-04-14 00:00:00', '1997-03-24 00:00:00', 3, 4.41000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10477, 'PRINI', 5, '1997-03-17 00:00:00', '1997-04-14 00:00:00', '1997-03-25 00:00:00', 2, 13.02000, 'Princesa Isabel Vinhos', 'Estrada da saúde n. 58', 'Lisboa', 'null', '1756', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10478, 'VICTE', 2, '1997-03-18 00:00:00', '1997-04-01 00:00:00', '1997-03-26 00:00:00', 3, 4.81000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10479, 'RATTC', 3, '1997-03-19 00:00:00', '1997-04-16 00:00:00', '1997-03-21 00:00:00', 3, 708.95000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10480, 'FOLIG', 6, '1997-03-20 00:00:00', '1997-04-17 00:00:00', '1997-03-24 00:00:00', 2, 1.35000, 'Folies gourmandes', '184, chaussée de Tournai', 'Lille', 'null', '59000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10481, 'RICAR', 8, '1997-03-20 00:00:00', '1997-04-17 00:00:00', '1997-03-25 00:00:00', 2, 64.33000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10482, 'LAZYK', 1, '1997-03-21 00:00:00', '1997-04-18 00:00:00', '1997-04-10 00:00:00', 3, 7.48000, 'Lazy K Kountry Store', '12 Orchestra Terrace', 'Walla Walla', 'WA', '99362', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10483, 'WHITC', 7, '1997-03-24 00:00:00', '1997-04-21 00:00:00', '1997-04-25 00:00:00', 2, 15.28000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10484, 'BSBEV', 3, '1997-03-24 00:00:00', '1997-04-21 00:00:00', '1997-04-01 00:00:00', 3, 6.88000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10485, 'LINOD', 4, '1997-03-25 00:00:00', '1997-04-08 00:00:00', '1997-03-31 00:00:00', 2, 64.45000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10486, 'HILAA', 1, '1997-03-26 00:00:00', '1997-04-23 00:00:00', '1997-04-02 00:00:00', 2, 30.53000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10487, 'QUEEN', 2, '1997-03-26 00:00:00', '1997-04-23 00:00:00', '1997-03-28 00:00:00', 2, 71.07000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10488, 'FRANK', 8, '1997-03-27 00:00:00', '1997-04-24 00:00:00', '1997-04-02 00:00:00', 2, 4.93000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10489, 'PICCO', 6, '1997-03-28 00:00:00', '1997-04-25 00:00:00', '1997-04-09 00:00:00', 2, 5.29000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10490, 'HILAA', 7, '1997-03-31 00:00:00', '1997-04-28 00:00:00', '1997-04-03 00:00:00', 2, 210.19000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10491, 'FURIB', 8, '1997-03-31 00:00:00', '1997-04-28 00:00:00', '1997-04-08 00:00:00', 3, 16.96000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10492, 'BOTTM', 3, '1997-04-01 00:00:00', '1997-04-29 00:00:00', '1997-04-11 00:00:00', 1, 62.89000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10493, 'LAMAI', 4, '1997-04-02 00:00:00', '1997-04-30 00:00:00', '1997-04-10 00:00:00', 3, 10.64000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10494, 'COMMI', 4, '1997-04-02 00:00:00', '1997-04-30 00:00:00', '1997-04-09 00:00:00', 2, 65.99000, 'Comércio Mineiro', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10495, 'LAUGB', 3, '1997-04-03 00:00:00', '1997-05-01 00:00:00', '1997-04-11 00:00:00', 3, 4.65000, 'Laughing Bacchus Wine Cellars', '2319 Elm St.', 'Vancouver', 'BC', 'V3F 2K1', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10496, 'TRADH', 7, '1997-04-04 00:00:00', '1997-05-02 00:00:00', '1997-04-07 00:00:00', 2, 46.77000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10497, 'LEHMS', 7, '1997-04-04 00:00:00', '1997-05-02 00:00:00', '1997-04-07 00:00:00', 1, 36.21000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10498, 'HILAA', 8, '1997-04-07 00:00:00', '1997-05-05 00:00:00', '1997-04-11 00:00:00', 2, 29.75000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10499, 'LILAS', 4, '1997-04-08 00:00:00', '1997-05-06 00:00:00', '1997-04-16 00:00:00', 2, 102.02000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10500, 'LAMAI', 6, '1997-04-09 00:00:00', '1997-05-07 00:00:00', '1997-04-17 00:00:00', 1, 42.68000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10501, 'BLAUS', 9, '1997-04-09 00:00:00', '1997-05-07 00:00:00', '1997-04-16 00:00:00', 3, 8.85000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10502, 'PERIC', 2, '1997-04-10 00:00:00', '1997-05-08 00:00:00', '1997-04-29 00:00:00', 1, 69.32000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10503, 'HUNGO', 6, '1997-04-11 00:00:00', '1997-05-09 00:00:00', '1997-04-16 00:00:00', 2, 16.74000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10504, 'WHITC', 4, '1997-04-11 00:00:00', '1997-05-09 00:00:00', '1997-04-18 00:00:00', 3, 59.13000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10505, 'MEREP', 3, '1997-04-14 00:00:00', '1997-05-12 00:00:00', '1997-04-21 00:00:00', 3, 7.13000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10506, 'KOENE', 9, '1997-04-15 00:00:00', '1997-05-13 00:00:00', '1997-05-02 00:00:00', 2, 21.19000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10507, 'ANTON', 7, '1997-04-15 00:00:00', '1997-05-13 00:00:00', '1997-04-22 00:00:00', 1, 47.45000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10508, 'OTTIK', 1, '1997-04-16 00:00:00', '1997-05-14 00:00:00', '1997-05-13 00:00:00', 2, 4.99000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10509, 'BLAUS', 4, '1997-04-17 00:00:00', '1997-05-15 00:00:00', '1997-04-29 00:00:00', 1, 0.15000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10510, 'SAVEA', 6, '1997-04-18 00:00:00', '1997-05-16 00:00:00', '1997-04-28 00:00:00', 3, 367.63000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10511, 'BONAP', 4, '1997-04-18 00:00:00', '1997-05-16 00:00:00', '1997-04-21 00:00:00', 3, 350.64000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10512, 'FAMIA', 7, '1997-04-21 00:00:00', '1997-05-19 00:00:00', '1997-04-24 00:00:00', 2, 3.53000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10513, 'WANDK', 7, '1997-04-22 00:00:00', '1997-06-03 00:00:00', '1997-04-28 00:00:00', 1, 105.65000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10514, 'ERNSH', 3, '1997-04-22 00:00:00', '1997-05-20 00:00:00', '1997-05-16 00:00:00', 2, 789.95000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10515, 'QUICK', 2, '1997-04-23 00:00:00', '1997-05-07 00:00:00', '1997-05-23 00:00:00', 1, 204.47000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10516, 'HUNGO', 2, '1997-04-24 00:00:00', '1997-05-22 00:00:00', '1997-05-01 00:00:00', 3, 62.78000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10517, 'NORTS', 3, '1997-04-24 00:00:00', '1997-05-22 00:00:00', '1997-04-29 00:00:00', 3, 32.07000, 'North/South', 'South House 300 Queensbridge', 'London', 'null', 'SW7 1RZ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10518, 'TORTU', 4, '1997-04-25 00:00:00', '1997-05-09 00:00:00', '1997-05-05 00:00:00', 2, 218.15000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10519, 'CHOPS', 6, '1997-04-28 00:00:00', '1997-05-26 00:00:00', '1997-05-01 00:00:00', 3, 91.76000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10520, 'SANTG', 7, '1997-04-29 00:00:00', '1997-05-27 00:00:00', '1997-05-01 00:00:00', 1, 13.37000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10521, 'CACTU', 8, '1997-04-29 00:00:00', '1997-05-27 00:00:00', '1997-05-02 00:00:00', 2, 17.22000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10522, 'LEHMS', 4, '1997-04-30 00:00:00', '1997-05-28 00:00:00', '1997-05-06 00:00:00', 1, 45.33000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10523, 'SEVES', 7, '1997-05-01 00:00:00', '1997-05-29 00:00:00', '1997-05-30 00:00:00', 2, 77.63000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10524, 'BERGS', 1, '1997-05-01 00:00:00', '1997-05-29 00:00:00', '1997-05-07 00:00:00', 2, 244.79000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10525, 'BONAP', 1, '1997-05-02 00:00:00', '1997-05-30 00:00:00', '1997-05-23 00:00:00', 2, 11.06000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10526, 'WARTH', 4, '1997-05-05 00:00:00', '1997-06-02 00:00:00', '1997-05-15 00:00:00', 2, 58.59000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10527, 'QUICK', 7, '1997-05-05 00:00:00', '1997-06-02 00:00:00', '1997-05-07 00:00:00', 1, 41.90000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10528, 'GREAL', 6, '1997-05-06 00:00:00', '1997-05-20 00:00:00', '1997-05-09 00:00:00', 2, 3.35000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10529, 'MAISD', 5, '1997-05-07 00:00:00', '1997-06-04 00:00:00', '1997-05-09 00:00:00', 2, 66.69000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10530, 'PICCO', 3, '1997-05-08 00:00:00', '1997-06-05 00:00:00', '1997-05-12 00:00:00', 2, 339.22000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10531, 'OCEAN', 7, '1997-05-08 00:00:00', '1997-06-05 00:00:00', '1997-05-19 00:00:00', 1, 8.12000, 'Océano Atlántico Ltda.', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10532, 'EASTC', 7, '1997-05-09 00:00:00', '1997-06-06 00:00:00', '1997-05-12 00:00:00', 3, 74.46000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10533, 'FOLKO', 8, '1997-05-12 00:00:00', '1997-06-09 00:00:00', '1997-05-22 00:00:00', 1, 188.04000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10534, 'LEHMS', 8, '1997-05-12 00:00:00', '1997-06-09 00:00:00', '1997-05-14 00:00:00', 2, 27.94000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10535, 'ANTON', 4, '1997-05-13 00:00:00', '1997-06-10 00:00:00', '1997-05-21 00:00:00', 1, 15.64000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10536, 'LEHMS', 3, '1997-05-14 00:00:00', '1997-06-11 00:00:00', '1997-06-06 00:00:00', 2, 58.88000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10537, 'RICSU', 1, '1997-05-14 00:00:00', '1997-05-28 00:00:00', '1997-05-19 00:00:00', 1, 78.85000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10538, 'BSBEV', 9, '1997-05-15 00:00:00', '1997-06-12 00:00:00', '1997-05-16 00:00:00', 3, 4.87000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10539, 'BSBEV', 6, '1997-05-16 00:00:00', '1997-06-13 00:00:00', '1997-05-23 00:00:00', 3, 12.36000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10540, 'QUICK', 3, '1997-05-19 00:00:00', '1997-06-16 00:00:00', '1997-06-13 00:00:00', 3, 1007.64000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10541, 'HANAR', 2, '1997-05-19 00:00:00', '1997-06-16 00:00:00', '1997-05-29 00:00:00', 1, 68.65000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10542, 'KOENE', 1, '1997-05-20 00:00:00', '1997-06-17 00:00:00', '1997-05-26 00:00:00', 3, 10.95000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10543, 'LILAS', 8, '1997-05-21 00:00:00', '1997-06-18 00:00:00', '1997-05-23 00:00:00', 2, 48.17000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10544, 'LONEP', 4, '1997-05-21 00:00:00', '1997-06-18 00:00:00', '1997-05-30 00:00:00', 1, 24.91000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10545, 'LAZYK', 8, '1997-05-22 00:00:00', '1997-06-19 00:00:00', '1997-06-26 00:00:00', 2, 11.92000, 'Lazy K Kountry Store', '12 Orchestra Terrace', 'Walla Walla', 'WA', '99362', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10546, 'VICTE', 1, '1997-05-23 00:00:00', '1997-06-20 00:00:00', '1997-05-27 00:00:00', 3, 194.72000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10547, 'SEVES', 3, '1997-05-23 00:00:00', '1997-06-20 00:00:00', '1997-06-02 00:00:00', 2, 178.43000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10548, 'TOMSP', 3, '1997-05-26 00:00:00', '1997-06-23 00:00:00', '1997-06-02 00:00:00', 2, 1.43000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10549, 'QUICK', 5, '1997-05-27 00:00:00', '1997-06-10 00:00:00', '1997-05-30 00:00:00', 1, 171.24000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10550, 'GODOS', 7, '1997-05-28 00:00:00', '1997-06-25 00:00:00', '1997-06-06 00:00:00', 3, 4.32000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10551, 'FURIB', 4, '1997-05-28 00:00:00', '1997-07-09 00:00:00', '1997-06-06 00:00:00', 3, 72.95000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10552, 'HILAA', 2, '1997-05-29 00:00:00', '1997-06-26 00:00:00', '1997-06-05 00:00:00', 1, 83.22000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10553, 'WARTH', 2, '1997-05-30 00:00:00', '1997-06-27 00:00:00', '1997-06-03 00:00:00', 2, 149.49000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10554, 'OTTIK', 4, '1997-05-30 00:00:00', '1997-06-27 00:00:00', '1997-06-05 00:00:00', 3, 120.97000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10555, 'SAVEA', 6, '1997-06-02 00:00:00', '1997-06-30 00:00:00', '1997-06-04 00:00:00', 3, 252.49000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10556, 'SIMOB', 2, '1997-06-03 00:00:00', '1997-07-15 00:00:00', '1997-06-13 00:00:00', 1, 9.80000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10557, 'LEHMS', 9, '1997-06-03 00:00:00', '1997-06-17 00:00:00', '1997-06-06 00:00:00', 2, 96.72000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10558, 'AROUT', 1, '1997-06-04 00:00:00', '1997-07-02 00:00:00', '1997-06-10 00:00:00', 2, 72.97000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10559, 'BLONP', 6, '1997-06-05 00:00:00', '1997-07-03 00:00:00', '1997-06-13 00:00:00', 1, 8.05000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10560, 'FRANK', 8, '1997-06-06 00:00:00', '1997-07-04 00:00:00', '1997-06-09 00:00:00', 1, 36.65000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10561, 'FOLKO', 2, '1997-06-06 00:00:00', '1997-07-04 00:00:00', '1997-06-09 00:00:00', 2, 242.21000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10562, 'REGGC', 1, '1997-06-09 00:00:00', '1997-07-07 00:00:00', '1997-06-12 00:00:00', 1, 22.95000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10563, 'RICAR', 2, '1997-06-10 00:00:00', '1997-07-22 00:00:00', '1997-06-24 00:00:00', 2, 60.43000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10564, 'RATTC', 4, '1997-06-10 00:00:00', '1997-07-08 00:00:00', '1997-06-16 00:00:00', 3, 13.75000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10565, 'MEREP', 8, '1997-06-11 00:00:00', '1997-07-09 00:00:00', '1997-06-18 00:00:00', 2, 7.15000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10566, 'BLONP', 9, '1997-06-12 00:00:00', '1997-07-10 00:00:00', '1997-06-18 00:00:00', 1, 88.40000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10567, 'HUNGO', 1, '1997-06-12 00:00:00', '1997-07-10 00:00:00', '1997-06-17 00:00:00', 1, 33.97000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10568, 'GALED', 3, '1997-06-13 00:00:00', '1997-07-11 00:00:00', '1997-07-09 00:00:00', 3, 6.54000, 'Galería del gastronómo', 'Rambla de Cataluña, 23', 'Barcelona', 'null', '8022', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10569, 'RATTC', 5, '1997-06-16 00:00:00', '1997-07-14 00:00:00', '1997-07-11 00:00:00', 1, 58.98000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10570, 'MEREP', 3, '1997-06-17 00:00:00', '1997-07-15 00:00:00', '1997-06-19 00:00:00', 3, 188.99000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10571, 'ERNSH', 8, '1997-06-17 00:00:00', '1997-07-29 00:00:00', '1997-07-04 00:00:00', 3, 26.06000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10572, 'BERGS', 3, '1997-06-18 00:00:00', '1997-07-16 00:00:00', '1997-06-25 00:00:00', 2, 116.43000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10573, 'ANTON', 7, '1997-06-19 00:00:00', '1997-07-17 00:00:00', '1997-06-20 00:00:00', 3, 84.84000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10574, 'TRAIH', 4, '1997-06-19 00:00:00', '1997-07-17 00:00:00', '1997-06-30 00:00:00', 2, 37.60000, 'Trail''s Head Gourmet Provisioners', '722 DaVinci Blvd.', 'Kirkland', 'WA', '98034', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10575, 'MORGK', 5, '1997-06-20 00:00:00', '1997-07-04 00:00:00', '1997-06-30 00:00:00', 1, 127.34000, 'Morgenstern Gesundkost', 'Heerstr. 22', 'Leipzig', 'null', '04179', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10576, 'TORTU', 3, '1997-06-23 00:00:00', '1997-07-07 00:00:00', '1997-06-30 00:00:00', 3, 18.56000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10577, 'TRAIH', 9, '1997-06-23 00:00:00', '1997-08-04 00:00:00', '1997-06-30 00:00:00', 2, 25.41000, 'Trail''s Head Gourmet Provisioners', '722 DaVinci Blvd.', 'Kirkland', 'WA', '98034', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10578, 'BSBEV', 4, '1997-06-24 00:00:00', '1997-07-22 00:00:00', '1997-07-25 00:00:00', 3, 29.60000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10579, 'LETSS', 1, '1997-06-25 00:00:00', '1997-07-23 00:00:00', '1997-07-04 00:00:00', 2, 13.73000, 'Let''s Stop N Shop', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10580, 'OTTIK', 4, '1997-06-26 00:00:00', '1997-07-24 00:00:00', '1997-07-01 00:00:00', 3, 75.89000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10581, 'FAMIA', 3, '1997-06-26 00:00:00', '1997-07-24 00:00:00', '1997-07-02 00:00:00', 1, 3.01000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10582, 'BLAUS', 3, '1997-06-27 00:00:00', '1997-07-25 00:00:00', '1997-07-14 00:00:00', 2, 27.71000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10583, 'WARTH', 2, '1997-06-30 00:00:00', '1997-07-28 00:00:00', '1997-07-04 00:00:00', 2, 7.28000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10584, 'BLONP', 4, '1997-06-30 00:00:00', '1997-07-28 00:00:00', '1997-07-04 00:00:00', 1, 59.14000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10585, 'WELLI', 7, '1997-07-01 00:00:00', '1997-07-29 00:00:00', '1997-07-10 00:00:00', 1, 13.41000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10586, 'REGGC', 9, '1997-07-02 00:00:00', '1997-07-30 00:00:00', '1997-07-09 00:00:00', 1, 0.48000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10587, 'QUEDE', 1, '1997-07-02 00:00:00', '1997-07-30 00:00:00', '1997-07-09 00:00:00', 1, 62.52000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10588, 'QUICK', 2, '1997-07-03 00:00:00', '1997-07-31 00:00:00', '1997-07-10 00:00:00', 3, 194.67000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10589, 'GREAL', 8, '1997-07-04 00:00:00', '1997-08-01 00:00:00', '1997-07-14 00:00:00', 2, 4.42000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10590, 'MEREP', 4, '1997-07-07 00:00:00', '1997-08-04 00:00:00', '1997-07-14 00:00:00', 3, 44.77000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10591, 'VAFFE', 1, '1997-07-07 00:00:00', '1997-07-21 00:00:00', '1997-07-16 00:00:00', 1, 55.92000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10592, 'LEHMS', 3, '1997-07-08 00:00:00', '1997-08-05 00:00:00', '1997-07-16 00:00:00', 1, 32.10000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10593, 'LEHMS', 7, '1997-07-09 00:00:00', '1997-08-06 00:00:00', '1997-08-13 00:00:00', 2, 174.20000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10594, 'OLDWO', 3, '1997-07-09 00:00:00', '1997-08-06 00:00:00', '1997-07-16 00:00:00', 2, 5.24000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10595, 'ERNSH', 2, '1997-07-10 00:00:00', '1997-08-07 00:00:00', '1997-07-14 00:00:00', 1, 96.78000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10596, 'WHITC', 8, '1997-07-11 00:00:00', '1997-08-08 00:00:00', '1997-08-12 00:00:00', 1, 16.34000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10597, 'PICCO', 7, '1997-07-11 00:00:00', '1997-08-08 00:00:00', '1997-07-18 00:00:00', 3, 35.12000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10598, 'RATTC', 1, '1997-07-14 00:00:00', '1997-08-11 00:00:00', '1997-07-18 00:00:00', 3, 44.42000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10599, 'BSBEV', 6, '1997-07-15 00:00:00', '1997-08-26 00:00:00', '1997-07-21 00:00:00', 3, 29.98000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10600, 'HUNGC', 4, '1997-07-16 00:00:00', '1997-08-13 00:00:00', '1997-07-21 00:00:00', 1, 45.13000, 'Hungry Coyote Import Store', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10601, 'HILAA', 7, '1997-07-16 00:00:00', '1997-08-27 00:00:00', '1997-07-22 00:00:00', 1, 58.30000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10602, 'VAFFE', 8, '1997-07-17 00:00:00', '1997-08-14 00:00:00', '1997-07-22 00:00:00', 2, 2.92000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10603, 'SAVEA', 8, '1997-07-18 00:00:00', '1997-08-15 00:00:00', '1997-08-08 00:00:00', 2, 48.77000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10604, 'FURIB', 1, '1997-07-18 00:00:00', '1997-08-15 00:00:00', '1997-07-29 00:00:00', 1, 7.46000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10605, 'MEREP', 1, '1997-07-21 00:00:00', '1997-08-18 00:00:00', '1997-07-29 00:00:00', 2, 379.13000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10606, 'TRADH', 4, '1997-07-22 00:00:00', '1997-08-19 00:00:00', '1997-07-31 00:00:00', 3, 79.40000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10607, 'SAVEA', 5, '1997-07-22 00:00:00', '1997-08-19 00:00:00', '1997-07-25 00:00:00', 1, 200.24000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10608, 'TOMSP', 4, '1997-07-23 00:00:00', '1997-08-20 00:00:00', '1997-08-01 00:00:00', 2, 27.79000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10609, 'DUMON', 7, '1997-07-24 00:00:00', '1997-08-21 00:00:00', '1997-07-30 00:00:00', 2, 1.85000, 'Du monde entier', '67, rue des Cinquante Otages', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10610, 'LAMAI', 8, '1997-07-25 00:00:00', '1997-08-22 00:00:00', '1997-08-06 00:00:00', 1, 26.78000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10611, 'WOLZA', 6, '1997-07-25 00:00:00', '1997-08-22 00:00:00', '1997-08-01 00:00:00', 2, 80.65000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10612, 'SAVEA', 1, '1997-07-28 00:00:00', '1997-08-25 00:00:00', '1997-08-01 00:00:00', 2, 544.08000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10613, 'HILAA', 4, '1997-07-29 00:00:00', '1997-08-26 00:00:00', '1997-08-01 00:00:00', 2, 8.11000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10614, 'BLAUS', 8, '1997-07-29 00:00:00', '1997-08-26 00:00:00', '1997-08-01 00:00:00', 3, 1.93000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10615, 'WILMK', 2, '1997-07-30 00:00:00', '1997-08-27 00:00:00', '1997-08-06 00:00:00', 3, 0.75000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10616, 'GREAL', 1, '1997-07-31 00:00:00', '1997-08-28 00:00:00', '1997-08-05 00:00:00', 2, 116.53000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10617, 'GREAL', 4, '1997-07-31 00:00:00', '1997-08-28 00:00:00', '1997-08-04 00:00:00', 2, 18.53000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10618, 'MEREP', 1, '1997-08-01 00:00:00', '1997-09-12 00:00:00', '1997-08-08 00:00:00', 1, 154.68000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10619, 'MEREP', 3, '1997-08-04 00:00:00', '1997-09-01 00:00:00', '1997-08-07 00:00:00', 3, 91.05000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10620, 'LAUGB', 2, '1997-08-05 00:00:00', '1997-09-02 00:00:00', '1997-08-14 00:00:00', 3, 0.94000, 'Laughing Bacchus Wine Cellars', '2319 Elm St.', 'Vancouver', 'BC', 'V3F 2K1', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10621, 'ISLAT', 4, '1997-08-05 00:00:00', '1997-09-02 00:00:00', '1997-08-11 00:00:00', 2, 23.73000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10622, 'RICAR', 4, '1997-08-06 00:00:00', '1997-09-03 00:00:00', '1997-08-11 00:00:00', 3, 50.97000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10623, 'FRANK', 8, '1997-08-07 00:00:00', '1997-09-04 00:00:00', '1997-08-12 00:00:00', 2, 97.18000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10624, 'THECR', 4, '1997-08-07 00:00:00', '1997-09-04 00:00:00', '1997-08-19 00:00:00', 2, 94.80000, 'The Cracker Box', '55 Grizzly Peak Rd.', 'Butte', 'MT', '59801', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10625, 'ANATR', 3, '1997-08-08 00:00:00', '1997-09-05 00:00:00', '1997-08-14 00:00:00', 1, 43.90000, 'Ana Trujillo Emparedados y helados', 'Avda. de la Constitución 2222', 'México D.F.', 'null', '05021', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10626, 'BERGS', 1, '1997-08-11 00:00:00', '1997-09-08 00:00:00', '1997-08-20 00:00:00', 2, 138.69000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10627, 'SAVEA', 8, '1997-08-11 00:00:00', '1997-09-22 00:00:00', '1997-08-21 00:00:00', 3, 107.46000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10628, 'BLONP', 4, '1997-08-12 00:00:00', '1997-09-09 00:00:00', '1997-08-20 00:00:00', 3, 30.36000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10629, 'GODOS', 4, '1997-08-12 00:00:00', '1997-09-09 00:00:00', '1997-08-20 00:00:00', 3, 85.46000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10630, 'KOENE', 1, '1997-08-13 00:00:00', '1997-09-10 00:00:00', '1997-08-19 00:00:00', 2, 32.35000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10631, 'LAMAI', 8, '1997-08-14 00:00:00', '1997-09-11 00:00:00', '1997-08-15 00:00:00', 1, 0.87000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10632, 'WANDK', 8, '1997-08-14 00:00:00', '1997-09-11 00:00:00', '1997-08-19 00:00:00', 1, 41.38000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10633, 'ERNSH', 7, '1997-08-15 00:00:00', '1997-09-12 00:00:00', '1997-08-18 00:00:00', 3, 477.90000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10634, 'FOLIG', 4, '1997-08-15 00:00:00', '1997-09-12 00:00:00', '1997-08-21 00:00:00', 3, 487.38000, 'Folies gourmandes', '184, chaussée de Tournai', 'Lille', 'null', '59000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10635, 'MAGAA', 8, '1997-08-18 00:00:00', '1997-09-15 00:00:00', '1997-08-21 00:00:00', 3, 47.46000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10636, 'WARTH', 4, '1997-08-19 00:00:00', '1997-09-16 00:00:00', '1997-08-26 00:00:00', 1, 1.15000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10637, 'QUEEN', 6, '1997-08-19 00:00:00', '1997-09-16 00:00:00', '1997-08-26 00:00:00', 1, 201.29000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10638, 'LINOD', 3, '1997-08-20 00:00:00', '1997-09-17 00:00:00', '1997-09-01 00:00:00', 1, 158.44000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10639, 'SANTG', 7, '1997-08-20 00:00:00', '1997-09-17 00:00:00', '1997-08-27 00:00:00', 3, 38.64000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10640, 'WANDK', 4, '1997-08-21 00:00:00', '1997-09-18 00:00:00', '1997-08-28 00:00:00', 1, 23.55000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10641, 'HILAA', 4, '1997-08-22 00:00:00', '1997-09-19 00:00:00', '1997-08-26 00:00:00', 2, 179.61000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10642, 'SIMOB', 7, '1997-08-22 00:00:00', '1997-09-19 00:00:00', '1997-09-05 00:00:00', 3, 41.89000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10643, 'ALFKI', 6, '1997-08-25 00:00:00', '1997-09-22 00:00:00', '1997-09-02 00:00:00', 1, 29.46000, 'Alfreds Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10644, 'WELLI', 3, '1997-08-25 00:00:00', '1997-09-22 00:00:00', '1997-09-01 00:00:00', 2, 0.14000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10645, 'HANAR', 4, '1997-08-26 00:00:00', '1997-09-23 00:00:00', '1997-09-02 00:00:00', 1, 12.41000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10646, 'HUNGO', 9, '1997-08-27 00:00:00', '1997-10-08 00:00:00', '1997-09-03 00:00:00', 3, 142.33000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10647, 'QUEDE', 4, '1997-08-27 00:00:00', '1997-09-10 00:00:00', '1997-09-03 00:00:00', 2, 45.54000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10648, 'RICAR', 5, '1997-08-28 00:00:00', '1997-10-09 00:00:00', '1997-09-09 00:00:00', 2, 14.25000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10649, 'MAISD', 5, '1997-08-28 00:00:00', '1997-09-25 00:00:00', '1997-08-29 00:00:00', 3, 6.20000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10650, 'FAMIA', 5, '1997-08-29 00:00:00', '1997-09-26 00:00:00', '1997-09-03 00:00:00', 3, 176.81000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10651, 'WANDK', 8, '1997-09-01 00:00:00', '1997-09-29 00:00:00', '1997-09-11 00:00:00', 2, 20.60000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10652, 'GOURL', 4, '1997-09-01 00:00:00', '1997-09-29 00:00:00', '1997-09-08 00:00:00', 2, 7.14000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10653, 'FRANK', 1, '1997-09-02 00:00:00', '1997-09-30 00:00:00', '1997-09-19 00:00:00', 1, 93.25000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10654, 'BERGS', 5, '1997-09-02 00:00:00', '1997-09-30 00:00:00', '1997-09-11 00:00:00', 1, 55.26000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10655, 'REGGC', 1, '1997-09-03 00:00:00', '1997-10-01 00:00:00', '1997-09-11 00:00:00', 2, 4.41000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10656, 'GREAL', 6, '1997-09-04 00:00:00', '1997-10-02 00:00:00', '1997-09-10 00:00:00', 1, 57.15000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10657, 'SAVEA', 2, '1997-09-04 00:00:00', '1997-10-02 00:00:00', '1997-09-15 00:00:00', 2, 352.69000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10658, 'QUICK', 4, '1997-09-05 00:00:00', '1997-10-03 00:00:00', '1997-09-08 00:00:00', 1, 364.15000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10659, 'QUEEN', 7, '1997-09-05 00:00:00', '1997-10-03 00:00:00', '1997-09-10 00:00:00', 2, 105.81000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10660, 'HUNGC', 8, '1997-09-08 00:00:00', '1997-10-06 00:00:00', '1997-10-15 00:00:00', 1, 111.29000, 'Hungry Coyote Import Store', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10661, 'HUNGO', 7, '1997-09-09 00:00:00', '1997-10-07 00:00:00', '1997-09-15 00:00:00', 3, 17.55000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10662, 'LONEP', 3, '1997-09-09 00:00:00', '1997-10-07 00:00:00', '1997-09-18 00:00:00', 2, 1.28000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10663, 'BONAP', 2, '1997-09-10 00:00:00', '1997-09-24 00:00:00', '1997-10-03 00:00:00', 2, 113.15000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10664, 'FURIB', 1, '1997-09-10 00:00:00', '1997-10-08 00:00:00', '1997-09-19 00:00:00', 3, 1.27000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10665, 'LONEP', 1, '1997-09-11 00:00:00', '1997-10-09 00:00:00', '1997-09-17 00:00:00', 2, 26.31000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10666, 'RICSU', 7, '1997-09-12 00:00:00', '1997-10-10 00:00:00', '1997-09-22 00:00:00', 2, 232.42000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10667, 'ERNSH', 7, '1997-09-12 00:00:00', '1997-10-10 00:00:00', '1997-09-19 00:00:00', 1, 78.09000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10668, 'WANDK', 1, '1997-09-15 00:00:00', '1997-10-13 00:00:00', '1997-09-23 00:00:00', 2, 47.22000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10669, 'SIMOB', 2, '1997-09-15 00:00:00', '1997-10-13 00:00:00', '1997-09-22 00:00:00', 1, 24.39000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10670, 'FRANK', 4, '1997-09-16 00:00:00', '1997-10-14 00:00:00', '1997-09-18 00:00:00', 1, 203.48000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10671, 'FRANR', 1, '1997-09-17 00:00:00', '1997-10-15 00:00:00', '1997-09-24 00:00:00', 1, 30.34000, 'France restauration', '54, rue Royale', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10672, 'BERGS', 9, '1997-09-17 00:00:00', '1997-10-01 00:00:00', '1997-09-26 00:00:00', 2, 95.75000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10673, 'WILMK', 2, '1997-09-18 00:00:00', '1997-10-16 00:00:00', '1997-09-19 00:00:00', 1, 22.76000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10674, 'ISLAT', 4, '1997-09-18 00:00:00', '1997-10-16 00:00:00', '1997-09-30 00:00:00', 2, 0.90000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10675, 'FRANK', 5, '1997-09-19 00:00:00', '1997-10-17 00:00:00', '1997-09-23 00:00:00', 2, 31.85000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10676, 'TORTU', 2, '1997-09-22 00:00:00', '1997-10-20 00:00:00', '1997-09-29 00:00:00', 2, 2.01000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10677, 'ANTON', 1, '1997-09-22 00:00:00', '1997-10-20 00:00:00', '1997-09-26 00:00:00', 3, 4.03000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10678, 'SAVEA', 7, '1997-09-23 00:00:00', '1997-10-21 00:00:00', '1997-10-16 00:00:00', 3, 388.98000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10679, 'BLONP', 8, '1997-09-23 00:00:00', '1997-10-21 00:00:00', '1997-09-30 00:00:00', 3, 27.94000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10680, 'OLDWO', 1, '1997-09-24 00:00:00', '1997-10-22 00:00:00', '1997-09-26 00:00:00', 1, 26.61000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10681, 'GREAL', 3, '1997-09-25 00:00:00', '1997-10-23 00:00:00', '1997-09-30 00:00:00', 3, 76.13000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10682, 'ANTON', 3, '1997-09-25 00:00:00', '1997-10-23 00:00:00', '1997-10-01 00:00:00', 2, 36.13000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10683, 'DUMON', 2, '1997-09-26 00:00:00', '1997-10-24 00:00:00', '1997-10-01 00:00:00', 1, 4.40000, 'Du monde entier', '67, rue des Cinquante Otages', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10684, 'OTTIK', 3, '1997-09-26 00:00:00', '1997-10-24 00:00:00', '1997-09-30 00:00:00', 1, 145.63000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10685, 'GOURL', 4, '1997-09-29 00:00:00', '1997-10-13 00:00:00', '1997-10-03 00:00:00', 2, 33.75000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10686, 'PICCO', 2, '1997-09-30 00:00:00', '1997-10-28 00:00:00', '1997-10-08 00:00:00', 1, 96.50000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10687, 'HUNGO', 9, '1997-09-30 00:00:00', '1997-10-28 00:00:00', '1997-10-30 00:00:00', 2, 296.43000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10688, 'VAFFE', 4, '1997-10-01 00:00:00', '1997-10-15 00:00:00', '1997-10-07 00:00:00', 2, 299.09000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10689, 'BERGS', 1, '1997-10-01 00:00:00', '1997-10-29 00:00:00', '1997-10-07 00:00:00', 2, 13.42000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10690, 'HANAR', 1, '1997-10-02 00:00:00', '1997-10-30 00:00:00', '1997-10-03 00:00:00', 1, 15.80000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10691, 'QUICK', 2, '1997-10-03 00:00:00', '1997-11-14 00:00:00', '1997-10-22 00:00:00', 2, 810.05000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10692, 'ALFKI', 4, '1997-10-03 00:00:00', '1997-10-31 00:00:00', '1997-10-13 00:00:00', 2, 61.02000, 'Alfred''s Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10693, 'WHITC', 3, '1997-10-06 00:00:00', '1997-10-20 00:00:00', '1997-10-10 00:00:00', 3, 139.34000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10694, 'QUICK', 8, '1997-10-06 00:00:00', '1997-11-03 00:00:00', '1997-10-09 00:00:00', 3, 398.36000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10695, 'WILMK', 7, '1997-10-07 00:00:00', '1997-11-18 00:00:00', '1997-10-14 00:00:00', 1, 16.72000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10696, 'WHITC', 8, '1997-10-08 00:00:00', '1997-11-19 00:00:00', '1997-10-14 00:00:00', 3, 102.55000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10697, 'LINOD', 3, '1997-10-08 00:00:00', '1997-11-05 00:00:00', '1997-10-14 00:00:00', 1, 45.52000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10698, 'ERNSH', 4, '1997-10-09 00:00:00', '1997-11-06 00:00:00', '1997-10-17 00:00:00', 1, 272.47000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10699, 'MORGK', 3, '1997-10-09 00:00:00', '1997-11-06 00:00:00', '1997-10-13 00:00:00', 3, 0.58000, 'Morgenstern Gesundkost', 'Heerstr. 22', 'Leipzig', 'null', '04179', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10700, 'SAVEA', 3, '1997-10-10 00:00:00', '1997-11-07 00:00:00', '1997-10-16 00:00:00', 1, 65.10000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10701, 'HUNGO', 6, '1997-10-13 00:00:00', '1997-10-27 00:00:00', '1997-10-15 00:00:00', 3, 220.31000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10702, 'ALFKI', 4, '1997-10-13 00:00:00', '1997-11-24 00:00:00', '1997-10-21 00:00:00', 1, 23.94000, 'Alfred''s Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10703, 'FOLKO', 6, '1997-10-14 00:00:00', '1997-11-11 00:00:00', '1997-10-20 00:00:00', 2, 152.30000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10704, 'QUEEN', 6, '1997-10-14 00:00:00', '1997-11-11 00:00:00', '1997-11-07 00:00:00', 1, 4.78000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10705, 'HILAA', 9, '1997-10-15 00:00:00', '1997-11-12 00:00:00', '1997-11-18 00:00:00', 2, 3.52000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10706, 'OLDWO', 8, '1997-10-16 00:00:00', '1997-11-13 00:00:00', '1997-10-21 00:00:00', 3, 135.63000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10707, 'AROUT', 4, '1997-10-16 00:00:00', '1997-10-30 00:00:00', '1997-10-23 00:00:00', 3, 21.74000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10708, 'THEBI', 6, '1997-10-17 00:00:00', '1997-11-28 00:00:00', '1997-11-05 00:00:00', 2, 2.96000, 'The Big Cheese', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10709, 'GOURL', 1, '1997-10-17 00:00:00', '1997-11-14 00:00:00', '1997-11-20 00:00:00', 3, 210.80000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10710, 'FRANS', 1, '1997-10-20 00:00:00', '1997-11-17 00:00:00', '1997-10-23 00:00:00', 1, 4.98000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10711, 'SAVEA', 5, '1997-10-21 00:00:00', '1997-12-02 00:00:00', '1997-10-29 00:00:00', 2, 52.41000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10712, 'HUNGO', 3, '1997-10-21 00:00:00', '1997-11-18 00:00:00', '1997-10-31 00:00:00', 1, 89.93000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10713, 'SAVEA', 1, '1997-10-22 00:00:00', '1997-11-19 00:00:00', '1997-10-24 00:00:00', 1, 167.05000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10714, 'SAVEA', 5, '1997-10-22 00:00:00', '1997-11-19 00:00:00', '1997-10-27 00:00:00', 3, 24.49000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10715, 'BONAP', 3, '1997-10-23 00:00:00', '1997-11-06 00:00:00', '1997-10-29 00:00:00', 1, 63.20000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10716, 'RANCH', 4, '1997-10-24 00:00:00', '1997-11-21 00:00:00', '1997-10-27 00:00:00', 2, 22.57000, 'Rancho grande', 'Av. del Libertador 900', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10717, 'FRANK', 1, '1997-10-24 00:00:00', '1997-11-21 00:00:00', '1997-10-29 00:00:00', 2, 59.25000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10718, 'KOENE', 1, '1997-10-27 00:00:00', '1997-11-24 00:00:00', '1997-10-29 00:00:00', 3, 170.88000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10719, 'LETSS', 8, '1997-10-27 00:00:00', '1997-11-24 00:00:00', '1997-11-05 00:00:00', 2, 51.44000, 'Let''s Stop N Shop', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10720, 'QUEDE', 8, '1997-10-28 00:00:00', '1997-11-11 00:00:00', '1997-11-05 00:00:00', 2, 9.53000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10721, 'QUICK', 5, '1997-10-29 00:00:00', '1997-11-26 00:00:00', '1997-10-31 00:00:00', 3, 48.92000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10722, 'SAVEA', 8, '1997-10-29 00:00:00', '1997-12-10 00:00:00', '1997-11-04 00:00:00', 1, 74.58000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10723, 'WHITC', 3, '1997-10-30 00:00:00', '1997-11-27 00:00:00', '1997-11-25 00:00:00', 1, 21.72000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10724, 'MEREP', 8, '1997-10-30 00:00:00', '1997-12-11 00:00:00', '1997-11-05 00:00:00', 2, 57.75000, 'Mère Paillarde', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10725, 'FAMIA', 4, '1997-10-31 00:00:00', '1997-11-28 00:00:00', '1997-11-05 00:00:00', 3, 10.83000, 'Familia Arquibaldo', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10726, 'EASTC', 4, '1997-11-03 00:00:00', '1997-11-17 00:00:00', '1997-12-05 00:00:00', 1, 16.56000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10727, 'REGGC', 2, '1997-11-03 00:00:00', '1997-12-01 00:00:00', '1997-12-05 00:00:00', 1, 89.90000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10728, 'QUEEN', 4, '1997-11-04 00:00:00', '1997-12-02 00:00:00', '1997-11-11 00:00:00', 2, 58.33000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10729, 'LINOD', 8, '1997-11-04 00:00:00', '1997-12-16 00:00:00', '1997-11-14 00:00:00', 3, 141.06000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10730, 'BONAP', 5, '1997-11-05 00:00:00', '1997-12-03 00:00:00', '1997-11-14 00:00:00', 1, 20.12000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10731, 'CHOPS', 7, '1997-11-06 00:00:00', '1997-12-04 00:00:00', '1997-11-14 00:00:00', 1, 96.65000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10732, 'BONAP', 3, '1997-11-06 00:00:00', '1997-12-04 00:00:00', '1997-11-07 00:00:00', 1, 16.97000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10733, 'BERGS', 1, '1997-11-07 00:00:00', '1997-12-05 00:00:00', '1997-11-10 00:00:00', 3, 110.11000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10734, 'GOURL', 2, '1997-11-07 00:00:00', '1997-12-05 00:00:00', '1997-11-12 00:00:00', 3, 1.63000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10735, 'LETSS', 6, '1997-11-10 00:00:00', '1997-12-08 00:00:00', '1997-11-21 00:00:00', 2, 45.97000, 'Let''s Stop N Shop', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10736, 'HUNGO', 9, '1997-11-11 00:00:00', '1997-12-09 00:00:00', '1997-11-21 00:00:00', 2, 44.10000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10737, 'VINET', 2, '1997-11-11 00:00:00', '1997-12-09 00:00:00', '1997-11-18 00:00:00', 2, 7.79000, 'Vins et alcools Chevalier', '59 rue de l''Abbaye', 'Reims', 'null', '51100', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10738, 'SPECD', 2, '1997-11-12 00:00:00', '1997-12-10 00:00:00', '1997-11-18 00:00:00', 1, 2.91000, 'Spécialités du monde', '25, rue Lauriston', 'Paris', 'null', '75016', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10739, 'VINET', 3, '1997-11-12 00:00:00', '1997-12-10 00:00:00', '1997-11-17 00:00:00', 3, 11.08000, 'Vins et alcools Chevalier', '59 rue de l''Abbaye', 'Reims', 'null', '51100', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10740, 'WHITC', 4, '1997-11-13 00:00:00', '1997-12-11 00:00:00', '1997-11-25 00:00:00', 2, 81.88000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10741, 'AROUT', 4, '1997-11-14 00:00:00', '1997-11-28 00:00:00', '1997-11-18 00:00:00', 3, 10.96000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10742, 'BOTTM', 3, '1997-11-14 00:00:00', '1997-12-12 00:00:00', '1997-11-18 00:00:00', 3, 243.73000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10743, 'AROUT', 1, '1997-11-17 00:00:00', '1997-12-15 00:00:00', '1997-11-21 00:00:00', 2, 23.72000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10744, 'VAFFE', 6, '1997-11-17 00:00:00', '1997-12-15 00:00:00', '1997-11-24 00:00:00', 1, 69.19000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10745, 'QUICK', 9, '1997-11-18 00:00:00', '1997-12-16 00:00:00', '1997-11-27 00:00:00', 1, 3.52000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10746, 'CHOPS', 1, '1997-11-19 00:00:00', '1997-12-17 00:00:00', '1997-11-21 00:00:00', 3, 31.43000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10747, 'PICCO', 6, '1997-11-19 00:00:00', '1997-12-17 00:00:00', '1997-11-26 00:00:00', 1, 117.33000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10748, 'SAVEA', 3, '1997-11-20 00:00:00', '1997-12-18 00:00:00', '1997-11-28 00:00:00', 1, 232.55000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10749, 'ISLAT', 4, '1997-11-20 00:00:00', '1997-12-18 00:00:00', '1997-12-19 00:00:00', 2, 61.53000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10750, 'WARTH', 9, '1997-11-21 00:00:00', '1997-12-19 00:00:00', '1997-11-24 00:00:00', 1, 79.30000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10751, 'RICSU', 3, '1997-11-24 00:00:00', '1997-12-22 00:00:00', '1997-12-03 00:00:00', 3, 130.79000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10752, 'NORTS', 2, '1997-11-24 00:00:00', '1997-12-22 00:00:00', '1997-11-28 00:00:00', 3, 1.39000, 'North/South', 'South House 300 Queensbridge', 'London', 'null', 'SW7 1RZ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10753, 'FRANS', 3, '1997-11-25 00:00:00', '1997-12-23 00:00:00', '1997-11-27 00:00:00', 1, 7.70000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10754, 'MAGAA', 6, '1997-11-25 00:00:00', '1997-12-23 00:00:00', '1997-11-27 00:00:00', 3, 2.38000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10755, 'BONAP', 4, '1997-11-26 00:00:00', '1997-12-24 00:00:00', '1997-11-28 00:00:00', 2, 16.71000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10756, 'SPLIR', 8, '1997-11-27 00:00:00', '1997-12-25 00:00:00', '1997-12-02 00:00:00', 2, 73.21000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10757, 'SAVEA', 6, '1997-11-27 00:00:00', '1997-12-25 00:00:00', '1997-12-15 00:00:00', 1, 8.19000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10758, 'RICSU', 3, '1997-11-28 00:00:00', '1997-12-26 00:00:00', '1997-12-04 00:00:00', 3, 138.17000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10759, 'ANATR', 3, '1997-11-28 00:00:00', '1997-12-26 00:00:00', '1997-12-12 00:00:00', 3, 11.99000, 'Ana Trujillo Emparedados y helados', 'Avda. de la Constitución 2222', 'México D.F.', 'null', '05021', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10760, 'MAISD', 4, '1997-12-01 00:00:00', '1997-12-29 00:00:00', '1997-12-10 00:00:00', 1, 155.64000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10761, 'RATTC', 5, '1997-12-02 00:00:00', '1997-12-30 00:00:00', '1997-12-08 00:00:00', 2, 18.66000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10762, 'FOLKO', 3, '1997-12-02 00:00:00', '1997-12-30 00:00:00', '1997-12-09 00:00:00', 1, 328.74000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10763, 'FOLIG', 3, '1997-12-03 00:00:00', '1997-12-31 00:00:00', '1997-12-08 00:00:00', 3, 37.35000, 'Folies gourmandes', '184, chaussée de Tournai', 'Lille', 'null', '59000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10764, 'ERNSH', 6, '1997-12-03 00:00:00', '1997-12-31 00:00:00', '1997-12-08 00:00:00', 3, 145.45000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10765, 'QUICK', 3, '1997-12-04 00:00:00', '1998-01-01 00:00:00', '1997-12-09 00:00:00', 3, 42.74000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10766, 'OTTIK', 4, '1997-12-05 00:00:00', '1998-01-02 00:00:00', '1997-12-09 00:00:00', 1, 157.55000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10767, 'SUPRD', 4, '1997-12-05 00:00:00', '1998-01-02 00:00:00', '1997-12-15 00:00:00', 3, 1.59000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10768, 'AROUT', 3, '1997-12-08 00:00:00', '1998-01-05 00:00:00', '1997-12-15 00:00:00', 2, 146.32000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10769, 'VAFFE', 3, '1997-12-08 00:00:00', '1998-01-05 00:00:00', '1997-12-12 00:00:00', 1, 65.06000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10770, 'HANAR', 8, '1997-12-09 00:00:00', '1998-01-06 00:00:00', '1997-12-17 00:00:00', 3, 5.32000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10771, 'ERNSH', 9, '1997-12-10 00:00:00', '1998-01-07 00:00:00', '1998-01-02 00:00:00', 2, 11.19000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10772, 'LEHMS', 3, '1997-12-10 00:00:00', '1998-01-07 00:00:00', '1997-12-19 00:00:00', 2, 91.28000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10773, 'ERNSH', 1, '1997-12-11 00:00:00', '1998-01-08 00:00:00', '1997-12-16 00:00:00', 3, 96.43000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10774, 'FOLKO', 4, '1997-12-11 00:00:00', '1997-12-25 00:00:00', '1997-12-12 00:00:00', 1, 48.20000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10775, 'THECR', 7, '1997-12-12 00:00:00', '1998-01-09 00:00:00', '1997-12-26 00:00:00', 1, 20.25000, 'The Cracker Box', '55 Grizzly Peak Rd.', 'Butte', 'MT', '59801', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10776, 'ERNSH', 1, '1997-12-15 00:00:00', '1998-01-12 00:00:00', '1997-12-18 00:00:00', 3, 351.53000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10777, 'GOURL', 7, '1997-12-15 00:00:00', '1997-12-29 00:00:00', '1998-01-21 00:00:00', 2, 3.01000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10778, 'BERGS', 3, '1997-12-16 00:00:00', '1998-01-13 00:00:00', '1997-12-24 00:00:00', 1, 6.79000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10779, 'MORGK', 3, '1997-12-16 00:00:00', '1998-01-13 00:00:00', '1998-01-14 00:00:00', 2, 58.13000, 'Morgenstern Gesundkost', 'Heerstr. 22', 'Leipzig', 'null', '04179', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10780, 'LILAS', 2, '1997-12-16 00:00:00', '1997-12-30 00:00:00', '1997-12-25 00:00:00', 1, 42.13000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10781, 'WARTH', 2, '1997-12-17 00:00:00', '1998-01-14 00:00:00', '1997-12-19 00:00:00', 3, 73.16000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10782, 'CACTU', 9, '1997-12-17 00:00:00', '1998-01-14 00:00:00', '1997-12-22 00:00:00', 3, 1.10000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10783, 'HANAR', 4, '1997-12-18 00:00:00', '1998-01-15 00:00:00', '1997-12-19 00:00:00', 2, 124.98000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10784, 'MAGAA', 4, '1997-12-18 00:00:00', '1998-01-15 00:00:00', '1997-12-22 00:00:00', 3, 70.09000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10785, 'GROSR', 1, '1997-12-18 00:00:00', '1998-01-15 00:00:00', '1997-12-24 00:00:00', 3, 1.51000, 'GROSELLA-Restaurante', '5ª Ave. Los Palos Grandes', 'Caracas', 'DF', '1081', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10786, 'QUEEN', 8, '1997-12-19 00:00:00', '1998-01-16 00:00:00', '1997-12-23 00:00:00', 1, 110.87000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10787, 'LAMAI', 2, '1997-12-19 00:00:00', '1998-01-02 00:00:00', '1997-12-26 00:00:00', 1, 249.93000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10788, 'QUICK', 1, '1997-12-22 00:00:00', '1998-01-19 00:00:00', '1998-01-19 00:00:00', 2, 42.70000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10789, 'FOLIG', 1, '1997-12-22 00:00:00', '1998-01-19 00:00:00', '1997-12-31 00:00:00', 2, 100.60000, 'Folies gourmandes', '184, chaussée de Tournai', 'Lille', 'null', '59000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10790, 'GOURL', 6, '1997-12-22 00:00:00', '1998-01-19 00:00:00', '1997-12-26 00:00:00', 1, 28.23000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10791, 'FRANK', 6, '1997-12-23 00:00:00', '1998-01-20 00:00:00', '1998-01-01 00:00:00', 2, 16.85000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10792, 'WOLZA', 1, '1997-12-23 00:00:00', '1998-01-20 00:00:00', '1997-12-31 00:00:00', 3, 23.79000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10793, 'AROUT', 3, '1997-12-24 00:00:00', '1998-01-21 00:00:00', '1998-01-08 00:00:00', 3, 4.52000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10794, 'QUEDE', 6, '1997-12-24 00:00:00', '1998-01-21 00:00:00', '1998-01-02 00:00:00', 1, 21.49000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10795, 'ERNSH', 8, '1997-12-24 00:00:00', '1998-01-21 00:00:00', '1998-01-20 00:00:00', 2, 126.66000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10796, 'HILAA', 3, '1997-12-25 00:00:00', '1998-01-22 00:00:00', '1998-01-14 00:00:00', 1, 26.52000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10797, 'DRACD', 7, '1997-12-25 00:00:00', '1998-01-22 00:00:00', '1998-01-05 00:00:00', 2, 33.35000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10798, 'ISLAT', 2, '1997-12-26 00:00:00', '1998-01-23 00:00:00', '1998-01-05 00:00:00', 1, 2.33000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10799, 'KOENE', 9, '1997-12-26 00:00:00', '1998-02-06 00:00:00', '1998-01-05 00:00:00', 3, 30.76000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10800, 'SEVES', 1, '1997-12-26 00:00:00', '1998-01-23 00:00:00', '1998-01-05 00:00:00', 3, 137.44000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10801, 'BOLID', 4, '1997-12-29 00:00:00', '1998-01-26 00:00:00', '1997-12-31 00:00:00', 2, 97.09000, 'Bólido Comidas preparadas', 'C/ Araquil, 67', 'Madrid', 'null', '28023', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10802, 'SIMOB', 4, '1997-12-29 00:00:00', '1998-01-26 00:00:00', '1998-01-02 00:00:00', 2, 257.26000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10803, 'WELLI', 4, '1997-12-30 00:00:00', '1998-01-27 00:00:00', '1998-01-06 00:00:00', 1, 55.23000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10804, 'SEVES', 6, '1997-12-30 00:00:00', '1998-01-27 00:00:00', '1998-01-07 00:00:00', 2, 27.33000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10805, 'THEBI', 2, '1997-12-30 00:00:00', '1998-01-27 00:00:00', '1998-01-09 00:00:00', 3, 237.34000, 'The Big Cheese', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10806, 'VICTE', 3, '1997-12-31 00:00:00', '1998-01-28 00:00:00', '1998-01-05 00:00:00', 2, 22.11000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10807, 'FRANS', 4, '1997-12-31 00:00:00', '1998-01-28 00:00:00', '1998-01-30 00:00:00', 1, 1.36000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10808, 'OLDWO', 2, '1998-01-01 00:00:00', '1998-01-29 00:00:00', '1998-01-09 00:00:00', 3, 45.53000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10809, 'WELLI', 7, '1998-01-01 00:00:00', '1998-01-29 00:00:00', '1998-01-07 00:00:00', 1, 4.87000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10810, 'LAUGB', 2, '1998-01-01 00:00:00', '1998-01-29 00:00:00', '1998-01-07 00:00:00', 3, 4.33000, 'Laughing Bacchus Wine Cellars', '2319 Elm St.', 'Vancouver', 'BC', 'V3F 2K1', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10811, 'LINOD', 8, '1998-01-02 00:00:00', '1998-01-30 00:00:00', '1998-01-08 00:00:00', 1, 31.22000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10812, 'REGGC', 5, '1998-01-02 00:00:00', '1998-01-30 00:00:00', '1998-01-12 00:00:00', 1, 59.78000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10813, 'RICAR', 1, '1998-01-05 00:00:00', '1998-02-02 00:00:00', '1998-01-09 00:00:00', 1, 47.38000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10814, 'VICTE', 3, '1998-01-05 00:00:00', '1998-02-02 00:00:00', '1998-01-14 00:00:00', 3, 130.94000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10815, 'SAVEA', 2, '1998-01-05 00:00:00', '1998-02-02 00:00:00', '1998-01-14 00:00:00', 3, 14.62000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10816, 'GREAL', 4, '1998-01-06 00:00:00', '1998-02-03 00:00:00', '1998-02-04 00:00:00', 2, 719.78000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10817, 'KOENE', 3, '1998-01-06 00:00:00', '1998-01-20 00:00:00', '1998-01-13 00:00:00', 2, 306.07000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10818, 'MAGAA', 7, '1998-01-07 00:00:00', '1998-02-04 00:00:00', '1998-01-12 00:00:00', 3, 65.48000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10819, 'CACTU', 2, '1998-01-07 00:00:00', '1998-02-04 00:00:00', '1998-01-16 00:00:00', 3, 19.76000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10820, 'RATTC', 3, '1998-01-07 00:00:00', '1998-02-04 00:00:00', '1998-01-13 00:00:00', 2, 37.52000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10821, 'SPLIR', 1, '1998-01-08 00:00:00', '1998-02-05 00:00:00', '1998-01-15 00:00:00', 1, 36.68000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10822, 'TRAIH', 6, '1998-01-08 00:00:00', '1998-02-05 00:00:00', '1998-01-16 00:00:00', 3, 7.00000, 'Trail''s Head Gourmet Provisioners', '722 DaVinci Blvd.', 'Kirkland', 'WA', '98034', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10823, 'LILAS', 5, '1998-01-09 00:00:00', '1998-02-06 00:00:00', '1998-01-13 00:00:00', 2, 163.97000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10824, 'FOLKO', 8, '1998-01-09 00:00:00', '1998-02-06 00:00:00', '1998-01-30 00:00:00', 1, 1.23000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10825, 'DRACD', 1, '1998-01-09 00:00:00', '1998-02-06 00:00:00', '1998-01-14 00:00:00', 1, 79.25000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10826, 'BLONP', 6, '1998-01-12 00:00:00', '1998-02-09 00:00:00', '1998-02-06 00:00:00', 1, 7.09000, 'Blondel père et fils', '24, place Kléber', 'Strasbourg', 'null', '67000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10827, 'BONAP', 1, '1998-01-12 00:00:00', '1998-01-26 00:00:00', '1998-02-06 00:00:00', 2, 63.54000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10828, 'RANCH', 9, '1998-01-13 00:00:00', '1998-01-27 00:00:00', '1998-02-04 00:00:00', 1, 90.85000, 'Rancho grande', 'Av. del Libertador 900', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10829, 'ISLAT', 9, '1998-01-13 00:00:00', '1998-02-10 00:00:00', '1998-01-23 00:00:00', 1, 154.72000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10830, 'TRADH', 4, '1998-01-13 00:00:00', '1998-02-24 00:00:00', '1998-01-21 00:00:00', 2, 81.83000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10831, 'SANTG', 3, '1998-01-14 00:00:00', '1998-02-11 00:00:00', '1998-01-23 00:00:00', 2, 72.19000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10832, 'LAMAI', 2, '1998-01-14 00:00:00', '1998-02-11 00:00:00', '1998-01-19 00:00:00', 2, 43.26000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10833, 'OTTIK', 6, '1998-01-15 00:00:00', '1998-02-12 00:00:00', '1998-01-23 00:00:00', 2, 71.49000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10834, 'TRADH', 1, '1998-01-15 00:00:00', '1998-02-12 00:00:00', '1998-01-19 00:00:00', 3, 29.78000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10835, 'ALFKI', 1, '1998-01-15 00:00:00', '1998-02-12 00:00:00', '1998-01-21 00:00:00', 3, 69.53000, 'Alfred''s Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10836, 'ERNSH', 7, '1998-01-16 00:00:00', '1998-02-13 00:00:00', '1998-01-21 00:00:00', 1, 411.88000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10837, 'BERGS', 9, '1998-01-16 00:00:00', '1998-02-13 00:00:00', '1998-01-23 00:00:00', 3, 13.32000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10838, 'LINOD', 3, '1998-01-19 00:00:00', '1998-02-16 00:00:00', '1998-01-23 00:00:00', 3, 59.28000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10839, 'TRADH', 3, '1998-01-19 00:00:00', '1998-02-16 00:00:00', '1998-01-22 00:00:00', 3, 35.43000, 'Tradiçao Hipermercados', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10840, 'LINOD', 4, '1998-01-19 00:00:00', '1998-03-02 00:00:00', '1998-02-16 00:00:00', 2, 2.71000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10841, 'SUPRD', 5, '1998-01-20 00:00:00', '1998-02-17 00:00:00', '1998-01-29 00:00:00', 2, 424.30000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10842, 'TORTU', 1, '1998-01-20 00:00:00', '1998-02-17 00:00:00', '1998-01-29 00:00:00', 3, 54.42000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10843, 'VICTE', 4, '1998-01-21 00:00:00', '1998-02-18 00:00:00', '1998-01-26 00:00:00', 2, 9.26000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10844, 'PICCO', 8, '1998-01-21 00:00:00', '1998-02-18 00:00:00', '1998-01-26 00:00:00', 2, 25.22000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10845, 'QUICK', 8, '1998-01-21 00:00:00', '1998-02-04 00:00:00', '1998-01-30 00:00:00', 1, 212.98000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10846, 'SUPRD', 2, '1998-01-22 00:00:00', '1998-03-05 00:00:00', '1998-01-23 00:00:00', 3, 56.46000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10847, 'SAVEA', 4, '1998-01-22 00:00:00', '1998-02-05 00:00:00', '1998-02-10 00:00:00', 3, 487.57000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10848, 'CONSH', 7, '1998-01-23 00:00:00', '1998-02-20 00:00:00', '1998-01-29 00:00:00', 2, 38.24000, 'Consolidated Holdings', 'Berkeley Gardens 12  Brewery', 'London', 'null', 'WX1 6LT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10849, 'KOENE', 9, '1998-01-23 00:00:00', '1998-02-20 00:00:00', '1998-01-30 00:00:00', 2, 0.56000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10850, 'VICTE', 1, '1998-01-23 00:00:00', '1998-03-06 00:00:00', '1998-01-30 00:00:00', 1, 49.19000, 'Victuailles en stock', '2, rue du Commerce', 'Lyon', 'null', '69004', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10851, 'RICAR', 5, '1998-01-26 00:00:00', '1998-02-23 00:00:00', '1998-02-02 00:00:00', 1, 160.55000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10852, 'RATTC', 8, '1998-01-26 00:00:00', '1998-02-09 00:00:00', '1998-01-30 00:00:00', 1, 174.05000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10853, 'BLAUS', 9, '1998-01-27 00:00:00', '1998-02-24 00:00:00', '1998-02-03 00:00:00', 2, 53.83000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10854, 'ERNSH', 3, '1998-01-27 00:00:00', '1998-02-24 00:00:00', '1998-02-05 00:00:00', 2, 100.22000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10855, 'OLDWO', 3, '1998-01-27 00:00:00', '1998-02-24 00:00:00', '1998-02-04 00:00:00', 1, 170.97000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10856, 'ANTON', 3, '1998-01-28 00:00:00', '1998-02-25 00:00:00', '1998-02-10 00:00:00', 2, 58.43000, 'Antonio Moreno Taquería', 'Mataderos  2312', 'México D.F.', 'null', '05023', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10857, 'BERGS', 8, '1998-01-28 00:00:00', '1998-02-25 00:00:00', '1998-02-06 00:00:00', 2, 188.85000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10858, 'LACOR', 2, '1998-01-29 00:00:00', '1998-02-26 00:00:00', '1998-02-03 00:00:00', 1, 52.51000, 'La corne d''abondance', '67, avenue de l''Europe', 'Versailles', 'null', '78000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10859, 'FRANK', 1, '1998-01-29 00:00:00', '1998-02-26 00:00:00', '1998-02-02 00:00:00', 2, 76.10000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10860, 'FRANR', 3, '1998-01-29 00:00:00', '1998-02-26 00:00:00', '1998-02-04 00:00:00', 3, 19.26000, 'France restauration', '54, rue Royale', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10861, 'WHITC', 4, '1998-01-30 00:00:00', '1998-02-27 00:00:00', '1998-02-17 00:00:00', 2, 14.93000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10862, 'LEHMS', 8, '1998-01-30 00:00:00', '1998-03-13 00:00:00', '1998-02-02 00:00:00', 2, 53.23000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10863, 'HILAA', 4, '1998-02-02 00:00:00', '1998-03-02 00:00:00', '1998-02-17 00:00:00', 2, 30.26000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10864, 'AROUT', 4, '1998-02-02 00:00:00', '1998-03-02 00:00:00', '1998-02-09 00:00:00', 2, 3.04000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10865, 'QUICK', 2, '1998-02-02 00:00:00', '1998-02-16 00:00:00', '1998-02-12 00:00:00', 1, 348.14000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10866, 'BERGS', 5, '1998-02-03 00:00:00', '1998-03-03 00:00:00', '1998-02-12 00:00:00', 1, 109.11000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10867, 'LONEP', 6, '1998-02-03 00:00:00', '1998-03-17 00:00:00', '1998-02-11 00:00:00', 1, 1.93000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10868, 'QUEEN', 7, '1998-02-04 00:00:00', '1998-03-04 00:00:00', '1998-02-23 00:00:00', 2, 191.27000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10869, 'SEVES', 5, '1998-02-04 00:00:00', '1998-03-04 00:00:00', '1998-02-09 00:00:00', 1, 143.28000, 'Seven Seas Imports', '90 Wadhurst Rd.', 'London', 'null', 'OX15 4NB', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10870, 'WOLZA', 5, '1998-02-04 00:00:00', '1998-03-04 00:00:00', '1998-02-13 00:00:00', 3, 12.04000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10871, 'BONAP', 9, '1998-02-05 00:00:00', '1998-03-05 00:00:00', '1998-02-10 00:00:00', 2, 112.27000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10872, 'GODOS', 5, '1998-02-05 00:00:00', '1998-03-05 00:00:00', '1998-02-09 00:00:00', 2, 175.32000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10873, 'WILMK', 4, '1998-02-06 00:00:00', '1998-03-06 00:00:00', '1998-02-09 00:00:00', 1, 0.82000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10874, 'GODOS', 5, '1998-02-06 00:00:00', '1998-03-06 00:00:00', '1998-02-11 00:00:00', 2, 19.58000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10875, 'BERGS', 4, '1998-02-06 00:00:00', '1998-03-06 00:00:00', '1998-03-03 00:00:00', 2, 32.37000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10876, 'BONAP', 7, '1998-02-09 00:00:00', '1998-03-09 00:00:00', '1998-02-12 00:00:00', 3, 60.42000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10877, 'RICAR', 1, '1998-02-09 00:00:00', '1998-03-09 00:00:00', '1998-02-19 00:00:00', 1, 38.06000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10878, 'QUICK', 4, '1998-02-10 00:00:00', '1998-03-10 00:00:00', '1998-02-12 00:00:00', 1, 46.69000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10879, 'WILMK', 3, '1998-02-10 00:00:00', '1998-03-10 00:00:00', '1998-02-12 00:00:00', 3, 8.50000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10880, 'FOLKO', 7, '1998-02-10 00:00:00', '1998-03-24 00:00:00', '1998-02-18 00:00:00', 1, 88.01000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10881, 'CACTU', 4, '1998-02-11 00:00:00', '1998-03-11 00:00:00', '1998-02-18 00:00:00', 1, 2.84000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10882, 'SAVEA', 4, '1998-02-11 00:00:00', '1998-03-11 00:00:00', '1998-02-20 00:00:00', 3, 23.10000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10883, 'LONEP', 8, '1998-02-12 00:00:00', '1998-03-12 00:00:00', '1998-02-20 00:00:00', 3, 0.53000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10884, 'LETSS', 4, '1998-02-12 00:00:00', '1998-03-12 00:00:00', '1998-02-13 00:00:00', 2, 90.97000, 'Let''s Stop N Shop', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10885, 'SUPRD', 6, '1998-02-12 00:00:00', '1998-03-12 00:00:00', '1998-02-18 00:00:00', 3, 5.64000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10886, 'HANAR', 1, '1998-02-13 00:00:00', '1998-03-13 00:00:00', '1998-03-02 00:00:00', 1, 4.99000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10887, 'GALED', 8, '1998-02-13 00:00:00', '1998-03-13 00:00:00', '1998-02-16 00:00:00', 3, 1.25000, 'Galería del gastronómo', 'Rambla de Cataluña, 23', 'Barcelona', 'null', '8022', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10888, 'GODOS', 1, '1998-02-16 00:00:00', '1998-03-16 00:00:00', '1998-02-23 00:00:00', 2, 51.87000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10889, 'RATTC', 9, '1998-02-16 00:00:00', '1998-03-16 00:00:00', '1998-02-23 00:00:00', 3, 280.61000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10890, 'DUMON', 7, '1998-02-16 00:00:00', '1998-03-16 00:00:00', '1998-02-18 00:00:00', 1, 32.76000, 'Du monde entier', '67, rue des Cinquante Otages', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10891, 'LEHMS', 7, '1998-02-17 00:00:00', '1998-03-17 00:00:00', '1998-02-19 00:00:00', 2, 20.37000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10892, 'MAISD', 4, '1998-02-17 00:00:00', '1998-03-17 00:00:00', '1998-02-19 00:00:00', 2, 120.27000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10893, 'KOENE', 9, '1998-02-18 00:00:00', '1998-03-18 00:00:00', '1998-02-20 00:00:00', 2, 77.78000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10894, 'SAVEA', 1, '1998-02-18 00:00:00', '1998-03-18 00:00:00', '1998-02-20 00:00:00', 1, 116.13000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10895, 'ERNSH', 3, '1998-02-18 00:00:00', '1998-03-18 00:00:00', '1998-02-23 00:00:00', 1, 162.75000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10896, 'MAISD', 7, '1998-02-19 00:00:00', '1998-03-19 00:00:00', '1998-02-27 00:00:00', 3, 32.45000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10897, 'HUNGO', 3, '1998-02-19 00:00:00', '1998-03-19 00:00:00', '1998-02-25 00:00:00', 2, 603.54000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10898, 'OCEAN', 4, '1998-02-20 00:00:00', '1998-03-20 00:00:00', '1998-03-06 00:00:00', 2, 1.27000, 'Océano Atlántico Ltda.', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10899, 'LILAS', 5, '1998-02-20 00:00:00', '1998-03-20 00:00:00', '1998-02-26 00:00:00', 3, 1.21000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10900, 'WELLI', 1, '1998-02-20 00:00:00', '1998-03-20 00:00:00', '1998-03-04 00:00:00', 2, 1.66000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10901, 'HILAA', 4, '1998-02-23 00:00:00', '1998-03-23 00:00:00', '1998-02-26 00:00:00', 1, 62.09000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10902, 'FOLKO', 1, '1998-02-23 00:00:00', '1998-03-23 00:00:00', '1998-03-03 00:00:00', 1, 44.15000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10903, 'HANAR', 3, '1998-02-24 00:00:00', '1998-03-24 00:00:00', '1998-03-04 00:00:00', 3, 36.71000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10904, 'WHITC', 3, '1998-02-24 00:00:00', '1998-03-24 00:00:00', '1998-02-27 00:00:00', 3, 162.95000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10905, 'WELLI', 9, '1998-02-24 00:00:00', '1998-03-24 00:00:00', '1998-03-06 00:00:00', 2, 13.72000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10906, 'WOLZA', 4, '1998-02-25 00:00:00', '1998-03-11 00:00:00', '1998-03-03 00:00:00', 3, 26.29000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10907, 'SPECD', 6, '1998-02-25 00:00:00', '1998-03-25 00:00:00', '1998-02-27 00:00:00', 3, 9.19000, 'Spécialités du monde', '25, rue Lauriston', 'Paris', 'null', '75016', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10908, 'REGGC', 4, '1998-02-26 00:01:00', '1998-03-26 00:00:00', '1998-03-06 00:00:00', 2, 32.96000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10909, 'SANTG', 1, '1998-02-26 00:00:00', '1998-03-26 00:00:00', '1998-03-10 00:00:00', 2, 53.05000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10910, 'WILMK', 1, '1998-02-26 00:00:00', '1998-03-26 00:00:00', '1998-03-04 00:00:00', 3, 38.11000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10911, 'GODOS', 3, '1998-02-26 00:00:00', '1998-03-26 00:00:00', '1998-03-05 00:00:00', 1, 38.19000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10912, 'HUNGO', 2, '1998-02-26 00:00:00', '1998-03-26 00:00:00', '1998-03-18 00:00:00', 2, 580.91000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10913, 'QUEEN', 4, '1998-02-26 00:00:00', '1998-03-26 00:00:00', '1998-03-04 00:00:00', 1, 33.05000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10914, 'QUEEN', 6, '1998-02-27 00:00:00', '1998-03-27 00:00:00', '1998-03-02 00:00:00', 1, 21.19000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10915, 'TORTU', 2, '1998-02-27 00:00:00', '1998-03-27 00:00:00', '1998-03-02 00:00:00', 2, 3.51000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10916, 'RANCH', 1, '1998-02-27 00:00:00', '1998-03-27 00:00:00', '1998-03-09 00:00:00', 2, 63.77000, 'Rancho grande', 'Av. del Libertador 900', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10917, 'ROMEY', 4, '1998-03-02 00:00:00', '1998-03-30 00:00:00', '1998-03-11 00:00:00', 2, 8.29000, 'Romero y tomillo', 'Gran Vía, 1', 'Madrid', 'null', '28001', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10918, 'BOTTM', 3, '1998-03-02 00:00:00', '1998-03-30 00:00:00', '1998-03-11 00:00:00', 3, 48.83000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10919, 'LINOD', 2, '1998-03-02 00:00:00', '1998-03-30 00:00:00', '1998-03-04 00:00:00', 2, 19.80000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10920, 'AROUT', 4, '1998-03-03 00:00:00', '1998-03-31 00:00:00', '1998-03-09 00:00:00', 2, 29.61000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10921, 'VAFFE', 1, '1998-03-03 00:00:00', '1998-04-14 00:00:00', '1998-03-09 00:00:00', 1, 176.48000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10922, 'HANAR', 5, '1998-03-03 00:00:00', '1998-03-31 00:00:00', '1998-03-05 00:00:00', 3, 62.74000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10923, 'LAMAI', 7, '1998-03-03 00:00:00', '1998-04-14 00:00:00', '1998-03-13 00:00:00', 3, 68.26000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10924, 'BERGS', 3, '1998-03-04 00:00:00', '1998-04-01 00:00:00', '1998-04-08 00:00:00', 2, 151.52000, 'Berglunds snabbköp', 'Berguvsvägen  8', 'Luleå', 'null', 'S-958 22', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10925, 'HANAR', 3, '1998-03-04 00:00:00', '1998-04-01 00:00:00', '1998-03-13 00:00:00', 1, 2.27000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10926, 'ANATR', 4, '1998-03-04 00:00:00', '1998-04-01 00:00:00', '1998-03-11 00:00:00', 3, 39.92000, 'Ana Trujillo Emparedados y helados', 'Avda. de la Constitución 2222', 'México D.F.', 'null', '05021', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10927, 'LACOR', 4, '1998-03-05 00:00:00', '1998-04-02 00:00:00', '1998-04-08 00:00:00', 1, 19.79000, 'La corne d''abondance', '67, avenue de l''Europe', 'Versailles', 'null', '78000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10928, 'GALED', 1, '1998-03-05 00:00:00', '1998-04-02 00:00:00', '1998-03-18 00:00:00', 1, 1.36000, 'Galería del gastronómo', 'Rambla de Cataluña, 23', 'Barcelona', 'null', '8022', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10929, 'FRANK', 6, '1998-03-05 00:00:00', '1998-04-02 00:00:00', '1998-03-12 00:00:00', 1, 33.93000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10930, 'SUPRD', 4, '1998-03-06 00:00:00', '1998-04-17 00:00:00', '1998-03-18 00:00:00', 3, 15.55000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10931, 'RICSU', 4, '1998-03-06 00:00:00', '1998-03-20 00:00:00', '1998-03-19 00:00:00', 2, 13.60000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10932, 'BONAP', 8, '1998-03-06 00:00:00', '1998-04-03 00:00:00', '1998-03-24 00:00:00', 1, 134.64000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10933, 'ISLAT', 6, '1998-03-06 00:00:00', '1998-04-03 00:00:00', '1998-03-16 00:00:00', 3, 54.15000, 'Island Trading', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10934, 'LEHMS', 3, '1998-03-09 00:00:00', '1998-04-06 00:00:00', '1998-03-12 00:00:00', 3, 32.01000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10935, 'WELLI', 4, '1998-03-09 00:00:00', '1998-04-06 00:00:00', '1998-03-18 00:00:00', 3, 47.59000, 'Wellington Importadora', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10936, 'GREAL', 3, '1998-03-09 00:00:00', '1998-04-06 00:00:00', '1998-03-18 00:00:00', 2, 33.68000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10937, 'CACTU', 7, '1998-03-10 00:00:00', '1998-03-24 00:00:00', '1998-03-13 00:00:00', 3, 31.51000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10938, 'QUICK', 3, '1998-03-10 00:00:00', '1998-04-07 00:00:00', '1998-03-16 00:00:00', 2, 31.89000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10939, 'MAGAA', 2, '1998-03-10 00:00:00', '1998-04-07 00:00:00', '1998-03-13 00:00:00', 2, 76.33000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10940, 'BONAP', 8, '1998-03-11 00:00:00', '1998-04-08 00:00:00', '1998-03-23 00:00:00', 3, 19.77000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10941, 'SAVEA', 7, '1998-03-11 00:00:00', '1998-04-08 00:00:00', '1998-03-20 00:00:00', 2, 400.81000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10942, 'REGGC', 9, '1998-03-11 00:00:00', '1998-04-08 00:00:00', '1998-03-18 00:00:00', 3, 17.95000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10943, 'BSBEV', 4, '1998-03-11 00:00:00', '1998-04-08 00:00:00', '1998-03-19 00:00:00', 2, 2.17000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10944, 'BOTTM', 6, '1998-03-12 00:00:00', '1998-03-26 00:00:00', '1998-03-13 00:00:00', 3, 52.92000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10945, 'MORGK', 4, '1998-03-12 00:00:00', '1998-04-09 00:00:00', '1998-03-18 00:00:00', 1, 10.22000, 'Morgenstern Gesundkost', 'Heerstr. 22', 'Leipzig', 'null', '04179', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10946, 'VAFFE', 1, '1998-03-12 00:00:00', '1998-04-09 00:00:00', '1998-03-19 00:00:00', 2, 27.20000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10947, 'BSBEV', 3, '1998-03-13 00:00:00', '1998-04-10 00:00:00', '1998-03-16 00:00:00', 2, 3.26000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10948, 'GODOS', 3, '1998-03-13 00:00:00', '1998-04-10 00:00:00', '1998-03-19 00:00:00', 3, 23.39000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10949, 'BOTTM', 2, '1998-03-13 00:00:00', '1998-04-10 00:00:00', '1998-03-17 00:00:00', 3, 74.44000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10950, 'MAGAA', 1, '1998-03-16 00:00:00', '1998-04-13 00:00:00', '1998-03-23 00:00:00', 2, 2.50000, 'Magazzini Alimentari Riuniti', 'Via Ludovico il Moro 22', 'Bergamo', 'null', '24100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10951, 'RICSU', 9, '1998-03-16 00:00:00', '1998-04-27 00:00:00', '1998-04-07 00:00:00', 2, 30.85000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10952, 'ALFKI', 1, '1998-03-16 00:00:00', '1998-04-27 00:00:00', '1998-03-24 00:00:00', 1, 40.42000, 'Alfred''s Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10953, 'AROUT', 9, '1998-03-16 00:00:00', '1998-03-30 00:00:00', '1998-03-25 00:00:00', 2, 23.72000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10954, 'LINOD', 5, '1998-03-17 00:00:00', '1998-04-28 00:00:00', '1998-03-20 00:00:00', 1, 27.91000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10955, 'FOLKO', 8, '1998-03-17 00:00:00', '1998-04-14 00:00:00', '1998-03-20 00:00:00', 2, 3.26000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10956, 'BLAUS', 6, '1998-03-17 00:00:00', '1998-04-28 00:00:00', '1998-03-20 00:00:00', 2, 44.65000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10957, 'HILAA', 8, '1998-03-18 00:00:00', '1998-04-15 00:00:00', '1998-03-27 00:00:00', 3, 105.36000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10958, 'OCEAN', 7, '1998-03-18 00:00:00', '1998-04-15 00:00:00', '1998-03-27 00:00:00', 2, 49.56000, 'Océano Atlántico Ltda.', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10959, 'GOURL', 6, '1998-03-18 00:00:00', '1998-04-29 00:00:00', '1998-03-23 00:00:00', 2, 4.98000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10960, 'HILAA', 3, '1998-03-19 00:00:00', '1998-04-02 00:00:00', '1998-04-08 00:00:00', 1, 2.08000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10961, 'QUEEN', 8, '1998-03-19 00:00:00', '1998-04-16 00:00:00', '1998-03-30 00:00:00', 1, 104.47000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10962, 'QUICK', 8, '1998-03-19 00:00:00', '1998-04-16 00:00:00', '1998-03-23 00:00:00', 2, 275.79000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10963, 'FURIB', 9, '1998-03-19 00:00:00', '1998-04-16 00:00:00', '1998-03-26 00:00:00', 3, 2.70000, 'Furia Bacalhau e Frutos do Mar', 'Jardim das rosas n. 32', 'Lisboa', 'null', '1675', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10964, 'SPECD', 3, '1998-03-20 00:00:00', '1998-04-17 00:00:00', '1998-03-24 00:00:00', 2, 87.38000, 'Spécialités du monde', '25, rue Lauriston', 'Paris', 'null', '75016', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10965, 'OLDWO', 6, '1998-03-20 00:00:00', '1998-04-17 00:00:00', '1998-03-30 00:00:00', 3, 144.38000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10966, 'CHOPS', 4, '1998-03-20 00:00:00', '1998-04-17 00:00:00', '1998-04-08 00:00:00', 1, 27.19000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10967, 'TOMSP', 2, '1998-03-23 00:00:00', '1998-04-20 00:00:00', '1998-04-02 00:00:00', 2, 62.22000, 'Toms Spezialitäten', 'Luisenstr. 48', 'Münster', 'null', '44087', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10968, 'ERNSH', 1, '1998-03-23 00:00:00', '1998-04-20 00:00:00', '1998-04-01 00:00:00', 3, 74.60000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10969, 'COMMI', 1, '1998-03-23 00:00:00', '1998-04-20 00:00:00', '1998-03-30 00:00:00', 2, 0.21000, 'Comércio Mineiro', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10970, 'BOLID', 9, '1998-03-24 00:00:00', '1998-04-07 00:00:00', '1998-04-24 00:00:00', 1, 16.16000, 'Bólido Comidas preparadas', 'C/ Araquil, 67', 'Madrid', 'null', '28023', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10971, 'FRANR', 2, '1998-03-24 00:00:00', '1998-04-21 00:00:00', '1998-04-02 00:00:00', 2, 121.82000, 'France restauration', '54, rue Royale', 'Nantes', 'null', '44000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10972, 'LACOR', 4, '1998-03-24 00:00:00', '1998-04-21 00:00:00', '1998-03-26 00:00:00', 2, 0.02000, 'La corne d''abondance', '67, avenue de l''Europe', 'Versailles', 'null', '78000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10973, 'LACOR', 6, '1998-03-24 00:00:00', '1998-04-21 00:00:00', '1998-03-27 00:00:00', 2, 15.17000, 'La corne d''abondance', '67, avenue de l''Europe', 'Versailles', 'null', '78000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10974, 'SPLIR', 3, '1998-03-25 00:00:00', '1998-04-08 00:00:00', '1998-04-03 00:00:00', 3, 12.96000, 'Split Rail Beer & Ale', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10975, 'BOTTM', 1, '1998-03-25 00:00:00', '1998-04-22 00:00:00', '1998-03-27 00:00:00', 3, 32.27000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10976, 'HILAA', 1, '1998-03-25 00:00:00', '1998-05-06 00:00:00', '1998-04-03 00:00:00', 1, 37.97000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10977, 'FOLKO', 8, '1998-03-26 00:00:00', '1998-04-23 00:00:00', '1998-04-10 00:00:00', 3, 208.50000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10978, 'MAISD', 9, '1998-03-26 00:00:00', '1998-04-23 00:00:00', '1998-04-23 00:00:00', 2, 32.82000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10979, 'ERNSH', 8, '1998-03-26 00:00:00', '1998-04-23 00:00:00', '1998-03-31 00:00:00', 2, 353.07000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10980, 'FOLKO', 4, '1998-03-27 00:00:00', '1998-05-08 00:00:00', '1998-04-17 00:00:00', 1, 1.26000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10981, 'HANAR', 1, '1998-03-27 00:00:00', '1998-04-24 00:00:00', '1998-04-02 00:00:00', 2, 193.37000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10982, 'BOTTM', 2, '1998-03-27 00:00:00', '1998-04-24 00:00:00', '1998-04-08 00:00:00', 1, 14.01000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10983, 'SAVEA', 2, '1998-03-27 00:00:00', '1998-04-24 00:00:00', '1998-04-06 00:00:00', 2, 657.54000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10984, 'SAVEA', 1, '1998-03-30 00:00:00', '1998-04-27 00:00:00', '1998-04-03 00:00:00', 3, 211.22000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10985, 'HUNGO', 2, '1998-03-30 00:00:00', '1998-04-27 00:00:00', '1998-04-02 00:00:00', 1, 91.51000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10986, 'OCEAN', 8, '1998-03-30 00:00:00', '1998-04-27 00:00:00', '1998-04-21 00:00:00', 2, 217.86000, 'Océano Atlántico Ltda.', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10987, 'EASTC', 8, '1998-03-31 00:00:00', '1998-04-28 00:00:00', '1998-04-06 00:00:00', 1, 185.48000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10988, 'RATTC', 3, '1998-03-31 00:00:00', '1998-04-28 00:00:00', '1998-04-10 00:00:00', 2, 61.14000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10989, 'QUEDE', 2, '1998-03-31 00:00:00', '1998-04-28 00:00:00', '1998-04-02 00:00:00', 1, 34.76000, 'Que Delícia', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10990, 'ERNSH', 2, '1998-04-01 00:00:00', '1998-05-13 00:00:00', '1998-04-07 00:00:00', 3, 117.61000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10991, 'QUICK', 1, '1998-04-01 00:00:00', '1998-04-29 00:00:00', '1998-04-07 00:00:00', 1, 38.51000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10992, 'THEBI', 1, '1998-04-01 00:00:00', '1998-04-29 00:00:00', '1998-04-03 00:00:00', 3, 4.27000, 'The Big Cheese', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10993, 'FOLKO', 7, '1998-04-01 00:00:00', '1998-04-29 00:00:00', '1998-04-10 00:00:00', 3, 8.81000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10994, 'VAFFE', 2, '1998-04-02 00:00:00', '1998-04-16 00:00:00', '1998-04-09 00:00:00', 3, 65.53000, 'Vaffeljernet', 'Smagsloget 45', 'Århus', 'null', '8200', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10995, 'PERIC', 1, '1998-04-02 00:00:00', '1998-04-30 00:00:00', '1998-04-06 00:00:00', 3, 46.00000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10996, 'QUICK', 4, '1998-04-02 00:00:00', '1998-04-30 00:00:00', '1998-04-10 00:00:00', 2, 1.12000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10997, 'LILAS', 8, '1998-04-03 00:00:00', '1998-05-15 00:00:00', '1998-04-13 00:00:00', 2, 73.91000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10998, 'WOLZA', 8, '1998-04-03 00:00:00', '1998-04-17 00:00:00', '1998-04-17 00:00:00', 2, 20.31000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (10999, 'OTTIK', 6, '1998-04-03 00:00:00', '1998-05-01 00:00:00', '1998-04-10 00:00:00', 2, 96.35000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11000, 'RATTC', 2, '1998-04-06 00:00:00', '1998-05-04 00:00:00', '1998-04-14 00:00:00', 3, 55.12000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11001, 'FOLKO', 2, '1998-04-06 00:00:00', '1998-05-04 00:00:00', '1998-04-14 00:00:00', 2, 197.30000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11002, 'SAVEA', 4, '1998-04-06 00:00:00', '1998-05-04 00:00:00', '1998-04-16 00:00:00', 1, 141.16000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11003, 'THECR', 3, '1998-04-06 00:00:00', '1998-05-04 00:00:00', '1998-04-08 00:00:00', 3, 14.91000, 'The Cracker Box', '55 Grizzly Peak Rd.', 'Butte', 'MT', '59801', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11004, 'MAISD', 3, '1998-04-07 00:00:00', '1998-05-05 00:00:00', '1998-04-20 00:00:00', 1, 44.84000, 'Maison Dewey', 'Rue Joseph-Bens 532', 'Bruxelles', 'null', 'B-1180', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11005, 'WILMK', 2, '1998-04-07 00:00:00', '1998-05-05 00:00:00', '1998-04-10 00:00:00', 1, 0.75000, 'Wilman Kala', 'Keskuskatu 45', 'Helsinki', 'null', '21240', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11006, 'GREAL', 3, '1998-04-07 00:00:00', '1998-05-05 00:00:00', '1998-04-15 00:00:00', 2, 25.19000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11007, 'PRINI', 8, '1998-04-08 00:00:00', '1998-05-06 00:00:00', '1998-04-13 00:00:00', 2, 202.24000, 'Princesa Isabel Vinhos', 'Estrada da saúde n. 58', 'Lisboa', 'null', '1756', 'Portugal');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11008, 'ERNSH', 7, '1998-04-08 00:00:00', '1998-05-06 00:00:00', NULL, 3, 79.46000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11009, 'GODOS', 2, '1998-04-08 00:00:00', '1998-05-06 00:00:00', '1998-04-10 00:00:00', 1, 59.11000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11010, 'REGGC', 2, '1998-04-09 00:00:00', '1998-05-07 00:00:00', '1998-04-21 00:00:00', 2, 28.71000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11011, 'ALFKI', 3, '1998-04-09 00:00:00', '1998-05-07 00:00:00', '1998-04-13 00:00:00', 1, 1.21000, 'Alfred''s Futterkiste', 'Obere Str. 57', 'Berlin', 'null', '12209', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11012, 'FRANK', 1, '1998-04-09 00:00:00', '1998-04-23 00:00:00', '1998-04-17 00:00:00', 3, 242.95000, 'Frankenversand', 'Berliner Platz 43', 'München', 'null', '80805', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11013, 'ROMEY', 2, '1998-04-09 00:00:00', '1998-05-07 00:00:00', '1998-04-10 00:00:00', 1, 32.99000, 'Romero y tomillo', 'Gran Vía, 1', 'Madrid', 'null', '28001', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11014, 'LINOD', 2, '1998-04-10 00:00:00', '1998-05-08 00:00:00', '1998-04-15 00:00:00', 3, 23.60000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11015, 'SANTG', 2, '1998-04-10 00:00:00', '1998-04-24 00:00:00', '1998-04-20 00:00:00', 2, 4.62000, 'Santé Gourmet', 'Erling Skakkes gate 78', 'Stavern', 'null', '4110', 'Norway');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11016, 'AROUT', 9, '1998-04-10 00:00:00', '1998-05-08 00:00:00', '1998-04-13 00:00:00', 2, 33.80000, 'Around the Horn', 'Brook Farm Stratford St. Mary', 'Colchester', 'Essex', 'CO7 6JX', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11017, 'ERNSH', 9, '1998-04-13 00:00:00', '1998-05-11 00:00:00', '1998-04-20 00:00:00', 2, 754.26000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11018, 'LONEP', 4, '1998-04-13 00:00:00', '1998-05-11 00:00:00', '1998-04-16 00:00:00', 2, 11.65000, 'Lonesome Pine Restaurant', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11019, 'RANCH', 6, '1998-04-13 00:00:00', '1998-05-11 00:00:00', NULL, 3, 3.17000, 'Rancho grande', 'Av. del Libertador 900', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11020, 'OTTIK', 2, '1998-04-14 00:00:00', '1998-05-12 00:00:00', '1998-04-16 00:00:00', 2, 43.30000, 'Ottilies Käseladen', 'Mehrheimerstr. 369', 'Köln', 'null', '50739', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11021, 'QUICK', 3, '1998-04-14 00:00:00', '1998-05-12 00:00:00', '1998-04-21 00:00:00', 1, 297.18000, 'QUICK-Stop', 'Taucherstraße 10', 'Cunewalde', 'null', '01307', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11022, 'HANAR', 9, '1998-04-14 00:00:00', '1998-05-12 00:00:00', '1998-05-04 00:00:00', 2, 6.27000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11023, 'BSBEV', 1, '1998-04-14 00:00:00', '1998-04-28 00:00:00', '1998-04-24 00:00:00', 2, 123.83000, 'B''s Beverages', 'Fauntleroy Circus', 'London', 'null', 'EC2 5NT', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11024, 'EASTC', 4, '1998-04-15 00:00:00', '1998-05-13 00:00:00', '1998-04-20 00:00:00', 1, 74.36000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11025, 'WARTH', 6, '1998-04-15 00:00:00', '1998-05-13 00:00:00', '1998-04-24 00:00:00', 3, 29.17000, 'Wartian Herkku', 'Torikatu 38', 'Oulu', 'null', '90110', 'Finland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11026, 'FRANS', 4, '1998-04-15 00:00:00', '1998-05-13 00:00:00', '1998-04-28 00:00:00', 1, 47.09000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11027, 'BOTTM', 1, '1998-04-16 00:00:00', '1998-05-14 00:00:00', '1998-04-20 00:00:00', 1, 52.52000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11028, 'KOENE', 2, '1998-04-16 00:00:00', '1998-05-14 00:00:00', '1998-04-22 00:00:00', 1, 29.59000, 'Königlich Essen', 'Maubelstr. 90', 'Brandenburg', 'null', '14776', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11029, 'CHOPS', 4, '1998-04-16 00:00:00', '1998-05-14 00:00:00', '1998-04-27 00:00:00', 1, 47.84000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11030, 'SAVEA', 7, '1998-04-17 00:00:00', '1998-05-15 00:00:00', '1998-04-27 00:00:00', 2, 830.75000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11031, 'SAVEA', 6, '1998-04-17 00:00:00', '1998-05-15 00:00:00', '1998-04-24 00:00:00', 2, 227.22000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11032, 'WHITC', 2, '1998-04-17 00:00:00', '1998-05-15 00:00:00', '1998-04-23 00:00:00', 3, 606.19000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11033, 'RICSU', 7, '1998-04-17 00:00:00', '1998-05-15 00:00:00', '1998-04-23 00:00:00', 3, 84.74000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11034, 'OLDWO', 8, '1998-04-20 00:00:00', '1998-06-01 00:00:00', '1998-04-27 00:00:00', 1, 40.32000, 'Old World Delicatessen', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11035, 'SUPRD', 2, '1998-04-20 00:00:00', '1998-05-18 00:00:00', '1998-04-24 00:00:00', 2, 0.17000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11036, 'DRACD', 8, '1998-04-20 00:00:00', '1998-05-18 00:00:00', '1998-04-22 00:00:00', 3, 149.47000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11037, 'GODOS', 7, '1998-04-21 00:00:00', '1998-05-19 00:00:00', '1998-04-27 00:00:00', 1, 3.20000, 'Godos Cocina Típica', 'C/ Romero, 33', 'Sevilla', 'null', '41101', 'Spain');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11038, 'SUPRD', 1, '1998-04-21 00:00:00', '1998-05-19 00:00:00', '1998-04-30 00:00:00', 2, 29.59000, 'Suprêmes délices', 'Boulevard Tirou, 255', 'Charleroi', 'null', 'B-6000', 'Belgium');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11039, 'LINOD', 1, '1998-04-21 00:00:00', '1998-05-19 00:00:00', NULL, 2, 65.00000, 'LINO-Delicateses', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11040, 'GREAL', 4, '1998-04-22 00:00:00', '1998-05-20 00:00:00', NULL, 3, 18.84000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11041, 'CHOPS', 3, '1998-04-22 00:00:00', '1998-05-20 00:00:00', '1998-04-28 00:00:00', 2, 48.22000, 'Chop-suey Chinese', 'Hauptstr. 31', 'Bern', 'null', '3012', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11042, 'COMMI', 2, '1998-04-22 00:00:00', '1998-05-06 00:00:00', '1998-05-01 00:00:00', 1, 29.99000, 'Comércio Mineiro', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11043, 'SPECD', 5, '1998-04-22 00:00:00', '1998-05-20 00:00:00', '1998-04-29 00:00:00', 2, 8.80000, 'Spécialités du monde', '25, rue Lauriston', 'Paris', 'null', '75016', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11044, 'WOLZA', 4, '1998-04-23 00:00:00', '1998-05-21 00:00:00', '1998-05-01 00:00:00', 1, 8.72000, 'Wolski Zajazd', 'ul. Filtrowa 68', 'Warszawa', 'null', '01-012', 'Poland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11045, 'BOTTM', 6, '1998-04-23 00:00:00', '1998-05-21 00:00:00', NULL, 2, 70.58000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11046, 'WANDK', 8, '1998-04-23 00:00:00', '1998-05-21 00:00:00', '1998-04-24 00:00:00', 2, 71.64000, 'Die Wandernde Kuh', 'Adenauerallee 900', 'Stuttgart', 'null', '70563', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11047, 'EASTC', 7, '1998-04-24 00:00:00', '1998-05-22 00:00:00', '1998-05-01 00:00:00', 3, 46.62000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11048, 'BOTTM', 7, '1998-04-24 00:00:00', '1998-05-22 00:00:00', '1998-04-30 00:00:00', 3, 24.12000, 'Bottom-Dollar Markets', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11049, 'GOURL', 3, '1998-04-24 00:00:00', '1998-05-22 00:00:00', '1998-05-04 00:00:00', 1, 8.34000, 'Gourmet Lanchonetes', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11050, 'FOLKO', 8, '1998-04-27 00:00:00', '1998-05-25 00:00:00', '1998-05-05 00:00:00', 2, 59.41000, 'Folk och fä HB', 'Åkergatan 24', 'Bräcke', 'null', 'S-844 67', 'Sweden');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11051, 'LAMAI', 7, '1998-04-27 00:00:00', '1998-05-25 00:00:00', NULL, 3, 2.79000, 'La maison d''Asie', '1 rue Alsace-Lorraine', 'Toulouse', 'null', '31000', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11052, 'HANAR', 3, '1998-04-27 00:00:00', '1998-05-25 00:00:00', '1998-05-01 00:00:00', 1, 67.26000, 'Hanari Carnes', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11053, 'PICCO', 2, '1998-04-27 00:00:00', '1998-05-25 00:00:00', '1998-04-29 00:00:00', 2, 53.05000, 'Piccolo und mehr', 'Geislweg 14', 'Salzburg', 'null', '5020', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11054, 'CACTU', 8, '1998-04-28 00:00:00', '1998-05-26 00:00:00', NULL, 1, 0.33000, 'Cactus Comidas para llevar', 'Cerrito 333', 'Buenos Aires', 'null', '1010', 'Argentina');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11055, 'HILAA', 7, '1998-04-28 00:00:00', '1998-05-26 00:00:00', '1998-05-05 00:00:00', 2, 120.92000, 'HILARION-Abastos', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11056, 'EASTC', 8, '1998-04-28 00:00:00', '1998-05-12 00:00:00', '1998-05-01 00:00:00', 2, 278.96000, 'Eastern Connection', '35 King George', 'London', 'null', 'WX3 6FW', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11057, 'NORTS', 3, '1998-04-29 00:00:00', '1998-05-27 00:00:00', '1998-05-01 00:00:00', 3, 4.13000, 'North/South', 'South House 300 Queensbridge', 'London', 'null', 'SW7 1RZ', 'UK');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11058, 'BLAUS', 9, '1998-04-29 00:00:00', '1998-05-27 00:00:00', NULL, 3, 31.14000, 'Blauer See Delikatessen', 'Forsterstr. 57', 'Mannheim', 'null', '68306', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11059, 'RICAR', 2, '1998-04-29 00:00:00', '1998-06-10 00:00:00', NULL, 2, 85.80000, 'Ricardo Adocicados', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11060, 'FRANS', 2, '1998-04-30 00:00:00', '1998-05-28 00:00:00', '1998-05-04 00:00:00', 2, 10.98000, 'Franchi S.p.A.', 'Via Monte Bianco 34', 'Torino', 'null', '10100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11061, 'GREAL', 4, '1998-04-30 00:00:00', '1998-06-11 00:00:00', NULL, 3, 14.01000, 'Great Lakes Food Market', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11062, 'REGGC', 4, '1998-04-30 00:00:00', '1998-05-28 00:00:00', NULL, 2, 29.93000, 'Reggiani Caseifici', 'Strada Provinciale 124', 'Reggio Emilia', 'null', '42100', 'Italy');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11063, 'HUNGO', 3, '1998-04-30 00:00:00', '1998-05-28 00:00:00', '1998-05-06 00:00:00', 2, 81.73000, 'Hungry Owl All-Night Grocers', '8 Johnstown Road', 'Cork', 'Co. Cork', 'null', 'Ireland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11064, 'SAVEA', 1, '1998-05-01 00:00:00', '1998-05-29 00:00:00', '1998-05-04 00:00:00', 1, 30.09000, 'Save-a-lot Markets', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11065, 'LILAS', 8, '1998-05-01 00:00:00', '1998-05-29 00:00:00', NULL, 1, 12.91000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11066, 'WHITC', 7, '1998-05-01 00:00:00', '1998-05-29 00:00:00', '1998-05-04 00:00:00', 2, 44.72000, 'White Clover Markets', '1029 - 12th Ave. S.', 'Seattle', 'WA', '98124', 'USA');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11067, 'DRACD', 1, '1998-05-04 00:00:00', '1998-05-18 00:00:00', '1998-05-06 00:00:00', 2, 7.98000, 'Drachenblut Delikatessen', 'Walserweg 21', 'Aachen', 'null', '52066', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11068, 'QUEEN', 8, '1998-05-04 00:00:00', '1998-06-01 00:00:00', NULL, 2, 81.75000, 'Queen Cozinha', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11069, 'TORTU', 1, '1998-05-04 00:00:00', '1998-06-01 00:00:00', '1998-05-06 00:00:00', 2, 15.67000, 'Tortuga Restaurante', 'Avda. Azteca 123', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11070, 'LEHMS', 2, '1998-05-05 00:00:00', '1998-06-02 00:00:00', NULL, 1, 136.00000, 'Lehmanns Marktstand', 'Magazinweg 7', 'Frankfurt a.M.', 'null', '60528', 'Germany');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11071, 'LILAS', 1, '1998-05-05 00:00:00', '1998-06-02 00:00:00', NULL, 1, 0.93000, 'LILA-Supermercado', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11072, 'ERNSH', 4, '1998-05-05 00:00:00', '1998-06-02 00:00:00', NULL, 2, 258.64000, 'Ernst Handel', 'Kirchgasse 6', 'Graz', 'null', '8010', 'Austria');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11073, 'PERIC', 2, '1998-05-05 00:00:00', '1998-06-02 00:00:00', NULL, 2, 24.95000, 'Pericles Comidas clásicas', 'Calle Dr. Jorge Cash 321', 'México D.F.', 'null', '05033', 'Mexico');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11074, 'SIMOB', 7, '1998-05-06 00:00:00', '1998-06-03 00:00:00', NULL, 2, 18.44000, 'Simons bistro', 'Vinbæltet 34', 'Kobenhavn', 'null', '1734', 'Denmark');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11075, 'RICSU', 8, '1998-05-06 00:00:00', '1998-06-03 00:00:00', NULL, 2, 6.19000, 'Richter Supermarkt', 'Starenweg 5', 'Genève', 'null', '1204', 'Switzerland');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11076, 'BONAP', 4, '1998-05-06 00:00:00', '1998-06-03 00:00:00', NULL, 2, 38.28000, 'Bon app''', '12, rue des Bouchers', 'Marseille', 'null', '13008', 'France');
INSERT INTO orders (orderid, customerid, employeeid, orderdate, requireddate, shippeddate, shipvia, freight, shipname, shipaddress, shipcity, shipregion, shippostalcode, shipcountry) VALUES (11077, 'RATTC', 1, '1998-05-06 00:00:00', '1998-06-03 00:00:00', NULL, 2, 8.53000, 'Rattlesnake Canyon Grocery', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA');


--
-- TOC entry 2007 (class 0 OID 29649)
-- Dependencies: 1609
-- Data for Name: patientrecords; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO patientrecords (patientrecordid, "Gender", "BirthDate", "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", stateid, "ZipCode", patientid) VALUES (1, 1, '1930-01-01 00:00:00', 'Bob', 'Barker', '123 Main St', NULL, 'New York', 1, '10001', 1);
INSERT INTO patientrecords (patientrecordid, "Gender", "BirthDate", "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", stateid, "ZipCode", patientid) VALUES (2, 1, '1969-01-01 00:00:00', 'John', 'Doe', '123 Main St', NULL, 'Tampa', 2, '33602', 2);
INSERT INTO patientrecords (patientrecordid, "Gender", "BirthDate", "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", stateid, "ZipCode", patientid) VALUES (3, 0, '1969-01-01 00:00:00', 'John', 'Doe', '123 Main St', 'Apt 2', 'Tampa', 2, '33602', 2);


--
-- TOC entry 2005 (class 0 OID 29633)
-- Dependencies: 1605
-- Data for Name: patients; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO patients (patientid, "Active", physicianid) VALUES (1, false, 1);
INSERT INTO patients (patientid, "Active", physicianid) VALUES (2, true, 2);


--
-- TOC entry 2006 (class 0 OID 29641)
-- Dependencies: 1607
-- Data for Name: physicians; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO physicians (physicianid, "Name") VALUES (1, 'Dr Dobbs');
INSERT INTO physicians (physicianid, "Name") VALUES (2, 'Dr Watson');


--
-- TOC entry 1987 (class 0 OID 29507)
-- Dependencies: 1578
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (1, 'Chai', 1, 1, '10 boxes x 20 bags', 18.00000, 39, 0, 10, false, 10.1);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (2, 'Chang', 1, 1, '24 - 12 oz bottles', 19.00000, 17, 40, 25, false, 23.142);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (3, 'Aniseed Syrup', 1, 2, '12 - 550 ml bottles', 10.00000, 13, 70, 25, false, 5.44);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (4, 'Chef Anton''s Cajun Seasoning', 2, 2, '48 - 6 oz jars', 22.00000, 53, 0, 0, false, 8.08);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (5, 'Chef Anton''s Gumbo Mix', 2, 2, '36 boxes', 21.35000, 0, 0, 0, true, 9.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (6, 'Grandma''s Boysenberry Spread', 3, 2, '12 - 8 oz jars', 25.00000, 120, 0, 25, false, 9.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (7, 'Uncle Bob''s Organic Dried Pears', 3, 7, '12 - 1 lb pkgs.', 30.00000, 15, 0, 10, false, 11.1);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (8, 'Northwoods Cranberry Sauce', 3, 2, '12 - 12 oz jars', 40.00000, 6, 0, 0, false, 11.01);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (9, 'Mishi Kobe Niku', 4, 6, '18 - 500 g pkgs.', 97.00000, 29, 0, 0, true, 3.022);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (10, 'Ikura', 4, 8, '12 - 200 ml jars', 31.00000, 31, 0, 0, false, 15.225);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (11, 'Queso Cabrales', 5, 4, '1 kg pkg.', 21.00000, 22, 30, 30, false, 13.3333);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (12, 'Queso Manchego La Pastora', 5, 4, '10 - 500 g pkgs.', 38.00000, 86, 0, 0, false, 9.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (13, 'Konbu', 6, 8, '2 kg box', 6.00000, 24, 0, 5, false, 8.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (14, 'Tofu', 6, 7, '40 - 100 g pkgs.', 23.25000, 35, 0, 0, false, 8.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (15, 'Genen Shouyu', 6, 2, '24 - 250 ml bottles', 15.50000, 39, 0, 5, false, 7.5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (16, 'Pavlova', 7, 3, '32 - 500 g boxes', 17.45000, 29, 0, 10, false, 6.25);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (17, 'Alice Mutton', 7, 6, '20 - 1 kg tins', 39.00000, 0, 0, 0, true, 8.755);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (18, 'Carnarvon Tigers', 7, 8, '16 kg pkg.', 62.50000, 42, 0, 0, false, 6.33);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (19, 'Teatime Chocolate Biscuits', 8, 3, '10 boxes x 12 pieces', 9.20000, 25, 0, 5, false, 4);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (20, 'Sir Rodney''s Marmalade', 8, 3, '30 gift boxes', 81.00000, 40, 0, 0, false, 2.22);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (21, 'Sir Rodney''s Scones', 8, 3, '24 pkgs. x 4 pieces', 10.00000, 3, 40, 5, false, 3.671);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (22, 'Gustaf''s Knäckebröd', 9, 5, '24 - 500 g pkgs.', 21.00000, 104, 0, 25, false, 7.77);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (23, 'Tunnbröd', 9, 5, '12 - 250 g pkgs.', 9.00000, 61, 0, 25, false, 9.98944);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (24, 'Guaraná Fantástica', 10, 1, '12 - 355 ml cans', 4.50000, 20, 0, 0, true, 4.5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (25, 'NuNuCa Nuß-Nougat-Creme', 11, 3, '20 - 450 g glasses', 14.00000, 76, 0, 30, false, 3.5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (26, 'Gumbär Gummibärchen', 11, 3, '100 - 250 g bags', 31.23000, 15, 0, 0, false, 8.88);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (27, 'Schoggi Schokolade', 11, 3, '100 - 100 g pieces', 43.90000, 49, 0, 30, false, 9.00);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (28, 'Rössle Sauerkraut', 12, 7, '25 - 825 g cans', 45.60000, 26, 0, 0, true, 10);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (29, 'Thüringer Rostbratwurst', 12, 6, '50 bags x 30 sausgs.', 123.79000, 0, 0, 0, true, 7.321);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (30, 'Nord-Ost Matjeshering', 13, 8, '10 - 200 g glasses', 25.89000, 10, 0, 15, false, 5.55);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (31, 'Gorgonzola Telino', 14, 4, '12 - 100 g pkgs', 12.50000, 0, 70, 20, false, 17.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (32, 'Mascarpone Fabioli', 14, 4, '24 - 200 g pkgs.', 32.00000, 9, 40, 25, false, 3);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (33, 'Geitost', 15, 4, '500 g', 2.50000, 112, 0, 20, false, 9);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (34, 'Sasquatch Ale', 16, 1, '24 - 12 oz bottles', 14.00000, 111, 0, 15, false, 3);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (35, 'Steeleye Stout', 16, 1, '24 - 12 oz bottles', 18.00000, 20, 0, 15, false, 4);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (36, 'Inlagd Sill', 17, 8, '24 - 250 g  jars', 19.00000, 112, 0, 20, false, 5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (37, 'Gravad lax', 17, 8, '12 - 500 g pkgs.', 26.00000, 11, 50, 25, false, 6);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (38, 'Côte de Blaye', 18, 1, '12 - 75 cl bottles', 263.50000, 17, 0, 15, false, 4.43);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (39, 'Chartreuse verte', 18, 1, '750 cc per bottle', 18.00000, 69, 0, 5, false, 7.99);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (40, 'Boston Crab Meat', 19, 8, '24 - 4 oz tins', 18.40000, 123, 0, 30, false, 9.77);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (41, 'Jack''s New England Clam Chowder', 19, 8, '12 - 12 oz cans', 9.65000, 85, 0, 10, false, 4.98);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (42, 'Singaporean Hokkien Fried Mee', 20, 5, '32 - 1 kg pkgs.', 14.00000, 26, 0, 0, true, 9.565);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (43, 'Ipoh Coffee', 20, 1, '16 - 500 g tins', 46.00000, 17, 10, 25, false, 6.555);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (44, 'Gula Malacca', 20, 2, '20 - 2 kg bags', 19.45000, 27, 0, 15, false, 5.111);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (45, 'Rogede sild', 21, 8, '1k pkg.', 9.50000, 5, 70, 15, false, 7.6);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (46, 'Spegesild', 21, 8, '4 - 450 g glasses', 12.00000, 95, 0, 0, false, 8.8);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (47, 'Zaanse koeken', 22, 3, '10 - 4 oz boxes', 9.50000, 36, 0, 0, false, 4.3);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (48, 'Chocolade', 22, 3, '10 pkgs.', 12.75000, 15, 70, 25, false, 5.5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (49, 'Maxilaku', 23, 3, '24 - 50 g pkgs.', 20.00000, 10, 60, 15, false, 6.6);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (50, 'Valkoinen suklaa', 23, 3, '12 - 100 g bars', 16.25000, 65, 0, 30, false, 8.888);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (51, 'Manjimup Dried Apples', 24, 7, '50 - 300 g pkgs.', 53.00000, 20, 0, 10, false, 7.654);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (52, 'Filo Mix', 24, 5, '16 - 2 kg boxes', 7.00000, 38, 0, 25, false, 1.2345);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (53, 'Perth Pasties', 24, 6, '48 pieces', 32.80000, 0, 0, 0, true, 9.12);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (54, 'Tourtière', 25, 6, '16 pies', 7.45000, 21, 0, 10, false, 5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (55, 'Pâté chinois', 25, 6, '24 boxes x 2 pies', 24.00000, 115, 0, 20, false, 6);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (56, 'Gnocchi di nonna Alice', 26, 5, '24 - 250 g pkgs.', 38.00000, 21, 10, 30, false, 7.0);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (57, 'Ravioli Angelo', 26, 5, '24 - 250 g pkgs.', 19.50000, 36, 0, 20, false, 8.22);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (58, 'Escargots de Bourgogne', 27, 8, '24 pieces', 13.25000, 62, 0, 20, false, 8.33);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (59, 'Raclette Courdavault', 28, 4, '5 kg pkg.', 55.00000, 79, 0, 0, false, 8.44);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (60, 'Camembert Pierrot', 28, 4, '15 - 300 g rounds', 34.00000, 19, 0, 0, false, 7.74);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (61, 'Sirop d''érable', 29, 2, '24 - 500 ml bottles', 28.50000, 113, 0, 25, false, 7.11111);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (62, 'Tarte au sucre', 29, 3, '48 pies', 49.30000, 17, 0, 0, false, 8.9123);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (63, 'Vegie-spread', 7, 2, '15 - 625 g jars', 43.90000, 24, 0, 5, false, 5);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (64, 'Wimmers gute Semmelknödel', 12, 5, '20 bags x 4 pieces', 33.25000, 22, 80, 30, false, 6);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (65, 'Louisiana Fiery Hot Pepper Sauce', 2, 2, '32 - 8 oz bottles', 21.05000, 76, 0, 0, false, 13.3);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (66, 'Louisiana Hot Spiced Okra', 2, 2, '24 - 8 oz jars', 17.00000, 4, 100, 20, false, 12.4);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (67, 'Laughing Lumberjack Lager', 16, 1, '24 - 12 oz bottles', 14.00000, 52, 0, 10, false, 7.6767);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (68, 'Scottish Longbreads', 8, 3, '10 boxes x 8 pieces', 12.50000, 6, 10, 15, false, 5.556);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (69, 'Gudbrandsdalsost', 15, 4, '10 kg pkg.', 36.00000, 26, 0, 15, false, 9.302);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (70, 'Outback Lager', 7, 1, '24 - 355 ml bottles', 15.00000, 15, 10, 30, false, 2.654);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (71, 'Flotemysost', 15, 4, '10 - 500 g pkgs.', 21.50000, 26, 0, 0, false, 9.375);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (72, 'Mozzarella di Giovanni', 14, 4, '24 - 200 g pkgs.', 34.80000, 14, 0, 0, false, 2);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (73, 'Röd Kaviar', 17, 8, '24 - 150 g jars', 15.00000, 101, 0, 5, false, 4);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (74, 'Longlife Tofu', 4, 7, '5 kg pkg.', 10.00000, 4, 20, 5, false, 7.139);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (75, 'Rhönbräu Klosterbier', 12, 1, '24 - 0.5 l bottles', 7.75000, 125, 0, 25, false, 6.667);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (76, 'Lakkalikööri', 23, 1, '500 ml', 18.00000, 57, 0, 20, false, 5.4);
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued, shippingweight) VALUES (77, 'Original Frankfurter grüne Soße', 12, 2, '12 boxes', 13.00000, 32, 0, 15, false, 3);

--
-- TOC entry 1989 (class 0 OID 29519)
-- Dependencies: 1581
-- Data for Name: region; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2000 (class 0 OID 29606)
-- Dependencies: 1599
-- Data for Name: reptile; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO reptile (animal, bodytemperature) VALUES (2, 14);
INSERT INTO reptile (animal, bodytemperature) VALUES (3, 18);


--
-- TOC entry 1994 (class 0 OID 29559)
-- Dependencies: 1589
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO roles (id, name, isactive, entityid, parentid) VALUES (1, 'Admin', true, 5, NULL);
INSERT INTO roles (id, name, isactive, entityid, parentid) VALUES (2, 'User', false, NULL, NULL);


--
-- TOC entry 1990 (class 0 OID 29525)
-- Dependencies: 1582
-- Data for Name: shippers; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO shippers (shipperid, companyname, phone, "Reference") VALUES (1, 'Speedy Express', '(503) 555-9831', '356e4a7e-b027-4321-ba40-e2677e6502cf');
INSERT INTO shippers (shipperid, companyname, phone, "Reference") VALUES (2, 'United Package', '(503) 555-3199', '6dfcd0d7-4d2e-4525-a502-3ea9aa52e965');
INSERT INTO shippers (shipperid, companyname, phone, "Reference") VALUES (3, 'Federal Shipping', '(503) 555-9931', '716f114b-e253-4166-8c76-46e6f340b58f');


--
-- TOC entry 2008 (class 0 OID 29660)
-- Dependencies: 1611
-- Data for Name: states; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO states (stateid, "Abbreviation", "FullName") VALUES (1, 'NY', 'New York');
INSERT INTO states (stateid, "Abbreviation", "FullName") VALUES (2, 'FL', 'Florida');


--
-- TOC entry 1991 (class 0 OID 29530)
-- Dependencies: 1583
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (1, 'Exotic Liquids', 'Charlotte Cooper', 'Purchasing Manager', '', '49 Gilbert St.', 'London', '', 'EC1 4SD', 'UK', '(171) 555-2222', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (2, 'New Orleans Cajun Delights', 'Shelley Burke', 'Order Administrator', '#CAJUN.HTM#', 'P.O. Box 78934', 'New Orleans', 'LA', '70117', 'USA', '(100) 555-4822', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (3, 'Grandma Kelly''s Homestead', 'Regina Murphy', 'Sales Representative', '', '707 Oxford Rd.', 'Ann Arbor', 'MI', '48104', 'USA', '(313) 555-5735', '(313) 555-3349');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (4, 'Tokyo Traders', 'Yoshi Nagase', 'Marketing Manager', '', '9-8 Sekimai Musashino-shi', 'Tokyo', '', '100', 'Japan', '(03) 3555-5011', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (5, 'Cooperativa de Quesos ''Las Cabras''', 'Antonio del Valle Saavedra', 'Export Administrator', '', 'Calle del Rosal 4', 'Oviedo', 'Asturias', '33007', 'Spain', '(98) 598 76 54', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (6, 'Mayumi''s', 'Mayumi Ohno', 'Marketing Representative', 'Mayumi''s (on the World Wide Web)#http://www.microsoft.com/accessdev/sampleapps/mayumi.htm#', '92 Setsuko Chuo-ku', 'Osaka', '', '545', 'Japan', '(06) 431-7877', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (7, 'Pavlova, Ltd.', 'Ian Devling', 'Marketing Manager', '', '74 Rose St. Moonie Ponds', 'Melbourne', 'Victoria', '3058', 'Australia', '(03) 444-2343', '(03) 444-6588');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (8, 'Specialty Biscuits, Ltd.', 'Peter Wilson', 'Sales Representative', '', '29 King''s Way', 'Manchester', '', 'M14 GSD', 'UK', '(161) 555-4448', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (9, 'PB Knäckebröd AB', 'Lars Peterson', 'Sales Agent', '', 'Kaloadagatan 13', 'Göteborg', '', 'S-345 67', 'Sweden', '031-987 65 43', '031-987 65 91');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (10, 'Refrescos Americanas LTDA', 'Carlos Diaz', 'Marketing Manager', '', 'Av. das Americanas 12.890', 'Sao Paulo', '', '5442', 'Brazil', '(11) 555 4640', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (11, 'Heli Süßwaren GmbH & Co. KG', 'Petra Winkler', 'Sales Manager', '', 'Tiergartenstraße 5', 'Berlin', '', '10785', 'Germany', '(010) 9984510', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (12, 'Plutzer Lebensmittelgroßmärkte AG', 'Martin Bein', 'International Marketing Mgr.', 'Plutzer (on the World Wide Web)#http://www.microsoft.com/accessdev/sampleapps/plutzer.htm#', 'Bogenallee 51', 'Frankfurt', '', '60439', 'Germany', '(069) 992755', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (13, 'Nord-Ost-Fisch Handelsgesellschaft mbH', 'Sven Petersen', 'Coordinator Foreign Markets', '', 'Frahmredder 112a', 'Cuxhaven', '', '27478', 'Germany', '(04721) 8713', '(04721) 8714');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (14, 'Formaggi Fortini s.r.l.', 'Elio Rossi', 'Sales Representative', '#FORMAGGI.HTM#', 'Viale Dante, 75', 'Ravenna', '', '48100', 'Italy', '(0544) 60323', '(0544) 60603');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (15, 'Norske Meierier', 'Beate Vileid', 'Marketing Manager', '', 'Hatlevegen 5', 'Sandvika', '', '1320', 'Norway', '(0)2-953010', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (16, 'Bigfoot Breweries', 'Cheryl Saylor', 'Regional Account Rep.', '', '3400 - 8th Avenue Suite 210', 'Bend', 'OR', '97101', 'USA', '(503) 555-9931', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (17, 'Svensk Sjöföda AB', 'Michael Björn', 'Sales Representative', '', 'Brovallavägen 231', 'Stockholm', '', 'S-123 45', 'Sweden', '08-123 45 67', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (18, 'Aux joyeux ecclésiastiques', 'Guylène Nodier', 'Sales Manager', '', '203, Rue des Francs-Bourgeois', 'Paris', '', '75004', 'France', '(1) 03.83.00.68', '(1) 03.83.00.62');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (19, 'New England Seafood Cannery', 'Robb Merchant', 'Wholesale Account Agent', '', 'Order Processing Dept. 2100 Paul Revere Blvd.', 'Boston', 'MA', '02134', 'USA', '(617) 555-3267', '(617) 555-3389');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (20, 'Leka Trading', 'Chandra Leka', 'Owner', '', '471 Serangoon Loop, Suite #402', 'Singapore', '', '0512', 'Singapore', '555-8787', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (21, 'Lyngbysild', 'Niels Petersen', 'Sales Manager', '', 'Lyngbysild Fiskebakken 10', 'Lyngby', '', '2800', 'Denmark', '43844108', '43844115');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (22, 'Zaanse Snoepfabriek', 'Dirk Luchte', 'Accounting Manager', '', 'Verkoop Rijnweg 22', 'Zaandam', '', '9999 ZZ', 'Netherlands', '(12345) 1212', '(12345) 1210');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (23, 'Karkki Oy', 'Anne Heikkonen', 'Product Manager', '', 'Valtakatu 12', 'Lappeenranta', '', '53120', 'Finland', '(953) 10956', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (24, 'G''day, Mate', 'Wendy Mackenzie', 'Sales Representative', 'G''day Mate (on the World Wide Web)#http://www.microsoft.com/accessdev/sampleapps/gdaymate.htm#', '170 Prince Edward Parade Hunter''s Hill', 'Sydney', 'NSW', '2042', 'Australia', '(02) 555-5914', '(02) 555-4873');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (25, 'Ma Maison', 'Jean-Guy Lauzon', 'Marketing Manager', '', '2960 Rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada', '(514) 555-9022', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (26, 'Pasta Buttini s.r.l.', 'Giovanni Giudici', 'Order Administrator', '', 'Via dei Gelsomini, 153', 'Salerno', '', '84100', 'Italy', '(089) 6547665', '(089) 6547667');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (27, 'Escargots Nouveaux', 'Marie Delamare', 'Sales Manager', '', '22, rue H. Voiron', 'Montceau', '', '71300', 'France', '85.57.00.07', '');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (28, 'Gai pâturage', 'Eliane Noz', 'Sales Representative', '', 'Bat. B 3, rue des Alpes', 'Annecy', '', '74000', 'France', '38.76.98.06', '38.76.98.58');
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, homepage, address, city, region, postalcode, country, phone, fax) VALUES (29, 'Forêts d''érables', 'Chantal Goulet', 'Accounting Manager', '', '148 rue Chasseur', 'Ste-Hyacinthe', 'Québec', 'J2S 7S8', 'Canada', '(514) 555-2955', '(514) 555-2921');


--
-- TOC entry 1992 (class 0 OID 29540)
-- Dependencies: 1585
-- Data for Name: territories; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 1998 (class 0 OID 29589)
-- Dependencies: 1596
-- Data for Name: timesheetentries; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (1, '2010-06-17 00:00:00', 6, 'testing 123', 2);
INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (2, '2010-06-18 00:00:00', 14, NULL, 2);
INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (3, '2011-02-28 22:23:19', 4, NULL, 3);
INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (4, '2011-02-28 22:13:19', 8, 'testing 456', 3);
INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (5, '2011-02-28 22:16:19', 7, NULL, 3);
INSERT INTO timesheetentries (timesheetentryid, entrydate, numberofhours, comments, timesheetid) VALUES (6, '2011-02-28 22:48:19', 38, NULL, 3);


--
-- TOC entry 1996 (class 0 OID 29578)
-- Dependencies: 1593
-- Data for Name: timesheets; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO timesheets (timesheetid, submitteddate, submitted) VALUES (1, '2010-06-17 00:00:00', true);
INSERT INTO timesheets (timesheetid, submitteddate, submitted) VALUES (2, '2010-06-16 00:00:00', false);
INSERT INTO timesheets (timesheetid, submitteddate, submitted) VALUES (3, '2011-03-01 22:03:19', true);


--
-- TOC entry 1997 (class 0 OID 29584)
-- Dependencies: 1594
-- Data for Name: timesheetusers; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO timesheetusers (timesheetid, userid) VALUES (1, 1);
INSERT INTO timesheetusers (timesheetid, userid) VALUES (1, 2);
INSERT INTO timesheetusers (timesheetid, userid) VALUES (2, 1);


--
-- TOC entry 1995 (class 0 OID 29567)
-- Dependencies: 1591
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO users (userid, name, invalidloginattempts, registeredat, lastlogindate, enum1, enum2, features, roleid, property1, property2, otherproperty1, createdbyid, modifiedbyid) VALUES (1, 'ayende', 4, '2010-06-17 00:00:00', NULL, 'Medium', 1, 0, 1, 'test1', 'test2', 'othertest1', 1, NULL);
INSERT INTO users (userid, name, invalidloginattempts, registeredat, lastlogindate, enum1, enum2, features, roleid, property1, property2, otherproperty1, createdbyid, modifiedbyid) VALUES (2, 'rahien', 5, '1998-12-31 00:00:00', NULL, 'Small', 0, 0, 2, NULL, 'test2', NULL, 1, NULL);
INSERT INTO users (userid, name, invalidloginattempts, registeredat, lastlogindate, enum1, enum2, features, roleid, property1, property2, otherproperty1, createdbyid, modifiedbyid) VALUES (3, 'nhibernate', 6, '2000-01-01 00:00:00', '2011-02-27 22:03:19', 'Medium', 0, 8, NULL, NULL, NULL, NULL, 1, NULL);


--
-- TOC entry 1935 (class 2606 OID 29605)
-- Dependencies: 1598 1598
-- Name: animal_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY animal
    ADD CONSTRAINT animal_pkey PRIMARY KEY (id);


--
-- TOC entry 1925 (class 2606 OID 29556)
-- Dependencies: 1587 1587
-- Name: anotherentity_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY anotherentity
    ADD CONSTRAINT anotherentity_pkey PRIMARY KEY (id);

ALTER TABLE ONLY compositeidentity
    ADD CONSTRAINT compositeidentity_pkey PRIMARY KEY (objectid, tenantid);

--
-- TOC entry 1945 (class 2606 OID 29630)
-- Dependencies: 1603 1603
-- Name: cat_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY cat
    ADD CONSTRAINT cat_pkey PRIMARY KEY (mammal);


--
-- TOC entry 1915 (class 2606 OID 29516)
-- Dependencies: 1579 1579
-- Name: categories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (categoryid);


--
-- TOC entry 1903 (class 2606 OID 29480)
-- Dependencies: 1572 1572
-- Name: customers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY customers
    ADD CONSTRAINT customers_pkey PRIMARY KEY (customerid);


--
-- TOC entry 1943 (class 2606 OID 29625)
-- Dependencies: 1602 1602
-- Name: dog_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY dog
    ADD CONSTRAINT dog_pkey PRIMARY KEY (mammal);


--
-- TOC entry 1905 (class 2606 OID 29488)
-- Dependencies: 1573 1573
-- Name: employees_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employeeid);


--
-- TOC entry 1907 (class 2606 OID 29493)
-- Dependencies: 1574 1574 1574
-- Name: employeeterritories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY employeeterritories
    ADD CONSTRAINT employeeterritories_pkey PRIMARY KEY (territoryid, employeeid);


--
-- TOC entry 1939 (class 2606 OID 29615)
-- Dependencies: 1600 1600
-- Name: lizard_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY lizard
    ADD CONSTRAINT lizard_pkey PRIMARY KEY (reptile);


--
-- TOC entry 1941 (class 2606 OID 29620)
-- Dependencies: 1601 1601
-- Name: mammal_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY mammal
    ADD CONSTRAINT mammal_pkey PRIMARY KEY (animal);


--
-- TOC entry 1911 (class 2606 OID 29506)
-- Dependencies: 1577 1577
-- Name: orderlines_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orderlines
    ADD CONSTRAINT orderlines_pkey PRIMARY KEY (orderlineid);


--
-- TOC entry 1909 (class 2606 OID 29498)
-- Dependencies: 1575 1575
-- Name: orders_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderid);


--
-- TOC entry 1951 (class 2606 OID 29657)
-- Dependencies: 1609 1609
-- Name: patientrecords_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY patientrecords
    ADD CONSTRAINT patientrecords_pkey PRIMARY KEY (patientrecordid);


--
-- TOC entry 1947 (class 2606 OID 29638)
-- Dependencies: 1605 1605
-- Name: patients_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY patients
    ADD CONSTRAINT patients_pkey PRIMARY KEY (patientid);


--
-- TOC entry 1949 (class 2606 OID 29646)
-- Dependencies: 1607 1607
-- Name: physicians_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY physicians
    ADD CONSTRAINT physicians_pkey PRIMARY KEY (physicianid);


--
-- TOC entry 1913 (class 2606 OID 29511)
-- Dependencies: 1578 1578
-- Name: products_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY products
    ADD CONSTRAINT products_pkey PRIMARY KEY (productid);


--
-- TOC entry 1917 (class 2606 OID 29524)
-- Dependencies: 1581 1581
-- Name: region_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY region
    ADD CONSTRAINT region_pkey PRIMARY KEY (regionid);


--
-- TOC entry 1937 (class 2606 OID 29610)
-- Dependencies: 1599 1599
-- Name: reptile_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY reptile
    ADD CONSTRAINT reptile_pkey PRIMARY KEY (animal);


--
-- TOC entry 1927 (class 2606 OID 29564)
-- Dependencies: 1589 1589
-- Name: roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 1919 (class 2606 OID 29529)
-- Dependencies: 1582 1582
-- Name: shippers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY shippers
    ADD CONSTRAINT shippers_pkey PRIMARY KEY (shipperid);


--
-- TOC entry 1953 (class 2606 OID 29668)
-- Dependencies: 1611 1611
-- Name: states_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY states
    ADD CONSTRAINT states_pkey PRIMARY KEY (stateid);


--
-- TOC entry 1921 (class 2606 OID 29537)
-- Dependencies: 1583 1583
-- Name: suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplierid);


--
-- TOC entry 1923 (class 2606 OID 29545)
-- Dependencies: 1585 1585
-- Name: territories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY territories
    ADD CONSTRAINT territories_pkey PRIMARY KEY (territoryid);


--
-- TOC entry 1933 (class 2606 OID 29594)
-- Dependencies: 1596 1596
-- Name: timesheetentries_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY timesheetentries
    ADD CONSTRAINT timesheetentries_pkey PRIMARY KEY (timesheetentryid);


--
-- TOC entry 1931 (class 2606 OID 29583)
-- Dependencies: 1593 1593
-- Name: timesheets_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY timesheets
    ADD CONSTRAINT timesheets_pkey PRIMARY KEY (timesheetid);


--
-- TOC entry 1929 (class 2606 OID 29575)
-- Dependencies: 1591 1591
-- Name: users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- TOC entry 1976 (class 2606 OID 29779)
-- Dependencies: 1598 1934 1601
-- Name: fk180fd9f3f5eb1539; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY mammal
    ADD CONSTRAINT fk180fd9f3f5eb1539 FOREIGN KEY (animal) REFERENCES animal(id);


--
-- TOC entry 1966 (class 2606 OID 29729)
-- Dependencies: 1589 1589 1926
-- Name: fk1a2e670f61069b3c; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT fk1a2e670f61069b3c FOREIGN KEY (parentid) REFERENCES roles(id);


--
-- TOC entry 1965 (class 2606 OID 29724)
-- Dependencies: 1924 1587 1589
-- Name: fk1a2e670fbf2fab1c; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT fk1a2e670fbf2fab1c FOREIGN KEY (entityid) REFERENCES anotherentity(id);

ALTER TABLE ONLY anotherentity
    ADD CONSTRAINT fk_anotherentity_compositeidentity FOREIGN KEY (compositeobjectid, compositetenantid) REFERENCES compositeidentity(objectid, tenantid);

--
-- TOC entry 1967 (class 2606 OID 29734)
-- Dependencies: 1926 1591 1589
-- Name: fk2c1c7fe527eb4ea8; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY users
    ADD CONSTRAINT fk2c1c7fe527eb4ea8 FOREIGN KEY (roleid) REFERENCES roles(id);


--
-- TOC entry 1958 (class 2606 OID 29689)
-- Dependencies: 1575 1904 1573
-- Name: fk318a099b4a64f51e; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orders
    ADD CONSTRAINT fk318a099b4a64f51e FOREIGN KEY (employeeid) REFERENCES employees(employeeid);


--
-- TOC entry 1957 (class 2606 OID 29684)
-- Dependencies: 1572 1575 1902
-- Name: fk318a099bbc41ebdf; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orders
    ADD CONSTRAINT fk318a099bbc41ebdf FOREIGN KEY (customerid) REFERENCES customers(customerid);


--
-- TOC entry 1959 (class 2606 OID 29694)
-- Dependencies: 1918 1575 1582
-- Name: fk318a099bcbef6a25; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orders
    ADD CONSTRAINT fk318a099bcbef6a25 FOREIGN KEY (shipvia) REFERENCES shippers(shipperid);


--
-- TOC entry 1981 (class 2606 OID 29804)
-- Dependencies: 1609 1605 1946
-- Name: fk43582e966423353d; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY patientrecords
    ADD CONSTRAINT fk43582e966423353d FOREIGN KEY (patientid) REFERENCES patients(patientid);


--
-- TOC entry 1980 (class 2606 OID 29799)
-- Dependencies: 1611 1609 1952
-- Name: fk43582e96a05ce37; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY patientrecords
    ADD CONSTRAINT fk43582e96a05ce37 FOREIGN KEY (stateid) REFERENCES states(stateid);


--
-- TOC entry 1963 (class 2606 OID 29714)
-- Dependencies: 1578 1914 1579
-- Name: fk4a7fd86a1abb59b6; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY products
    ADD CONSTRAINT fk4a7fd86a1abb59b6 FOREIGN KEY (categoryid) REFERENCES categories(categoryid);


--
-- TOC entry 1962 (class 2606 OID 29709)
-- Dependencies: 1583 1578 1920
-- Name: fk4a7fd86a4cac42f1; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY products
    ADD CONSTRAINT fk4a7fd86a4cac42f1 FOREIGN KEY (supplierid) REFERENCES suppliers(supplierid);


--
-- TOC entry 1979 (class 2606 OID 29794)
-- Dependencies: 1607 1948 1605
-- Name: fk66884859a7f1a2a3; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY patients
    ADD CONSTRAINT fk66884859a7f1a2a3 FOREIGN KEY (physicianid) REFERENCES physicians(physicianid);


--
-- TOC entry 1970 (class 2606 OID 29749)
-- Dependencies: 1593 1930 1596
-- Name: fk7e2220507850fdd8; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY timesheetentries
    ADD CONSTRAINT fk7e2220507850fdd8 FOREIGN KEY (timesheetid) REFERENCES timesheets(timesheetid);


--
-- TOC entry 1973 (class 2606 OID 29764)
-- Dependencies: 1598 1598 1934
-- Name: fk8ded074324e1a0d7; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY animal
    ADD CONSTRAINT fk8ded074324e1a0d7 FOREIGN KEY (parentid) REFERENCES animal(id);


--
-- TOC entry 1972 (class 2606 OID 29759)
-- Dependencies: 1934 1598 1598
-- Name: fk8ded07435db28373; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY animal
    ADD CONSTRAINT fk8ded07435db28373 FOREIGN KEY (father_id) REFERENCES animal(id);


--
-- TOC entry 1971 (class 2606 OID 29754)
-- Dependencies: 1598 1598 1934
-- Name: fk8ded07436534da5e; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY animal
    ADD CONSTRAINT fk8ded07436534da5e FOREIGN KEY (mother_id) REFERENCES animal(id);


--
-- TOC entry 1974 (class 2606 OID 29769)
-- Dependencies: 1599 1598 1934
-- Name: fk93d08bcaf5eb1539; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY reptile
    ADD CONSTRAINT fk93d08bcaf5eb1539 FOREIGN KEY (animal) REFERENCES animal(id);


--
-- TOC entry 1978 (class 2606 OID 29789)
-- Dependencies: 1603 1940 1601
-- Name: fk98330323ff33c879; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY cat
    ADD CONSTRAINT fk98330323ff33c879 FOREIGN KEY (mammal) REFERENCES mammal(animal);


--
-- TOC entry 1960 (class 2606 OID 29699)
-- Dependencies: 1575 1908 1577
-- Name: fk9d532a8f561978af; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orderlines
    ADD CONSTRAINT fk9d532a8f561978af FOREIGN KEY (orderid) REFERENCES orders(orderid);


--
-- TOC entry 1961 (class 2606 OID 29704)
-- Dependencies: 1912 1578 1577
-- Name: fk9d532a8f90fef4a2; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY orderlines
    ADD CONSTRAINT fk9d532a8f90fef4a2 FOREIGN KEY (productid) REFERENCES products(productid);


--
-- TOC entry 1969 (class 2606 OID 29744)
-- Dependencies: 1593 1930 1594
-- Name: fka6eef7377850fdd8; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY timesheetusers
    ADD CONSTRAINT fka6eef7377850fdd8 FOREIGN KEY (timesheetid) REFERENCES timesheets(timesheetid);


--
-- TOC entry 1968 (class 2606 OID 29739)
-- Dependencies: 1594 1591 1928
-- Name: fka6eef737bf902716; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY timesheetusers
    ADD CONSTRAINT fka6eef737bf902716 FOREIGN KEY (userid) REFERENCES users(userid);


--
-- TOC entry 1977 (class 2606 OID 29784)
-- Dependencies: 1602 1601 1940
-- Name: fkaaa2aaa3ff33c879; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY dog
    ADD CONSTRAINT fkaaa2aaa3ff33c879 FOREIGN KEY (mammal) REFERENCES mammal(animal);


--
-- TOC entry 1954 (class 2606 OID 29669)
-- Dependencies: 1573 1573 1904
-- Name: fkbf2d7ff2fbd4674; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY employees
    ADD CONSTRAINT fkbf2d7ff2fbd4674 FOREIGN KEY (reportsto) REFERENCES employees(employeeid);


--
-- TOC entry 1975 (class 2606 OID 29774)
-- Dependencies: 1600 1936 1599
-- Name: fkd8de3264ff7212d2; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY lizard
    ADD CONSTRAINT fkd8de3264ff7212d2 FOREIGN KEY (reptile) REFERENCES reptile(animal);


--
-- TOC entry 1964 (class 2606 OID 29719)
-- Dependencies: 1916 1581 1585
-- Name: fke8c9ac09ddf6d57a; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY territories
    ADD CONSTRAINT fke8c9ac09ddf6d57a FOREIGN KEY (regionid) REFERENCES region(regionid);


--
-- TOC entry 1956 (class 2606 OID 29679)
-- Dependencies: 1904 1573 1574
-- Name: fkf4419a394a64f51e; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY employeeterritories
    ADD CONSTRAINT fkf4419a394a64f51e FOREIGN KEY (employeeid) REFERENCES employees(employeeid);


--
-- TOC entry 1955 (class 2606 OID 29674)
-- Dependencies: 1574 1922 1585
-- Name: fkf4419a39a15f8729; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY employeeterritories
    ADD CONSTRAINT fkf4419a39a15f8729 FOREIGN KEY (territoryid) REFERENCES territories(territoryid);


-- Completed on 2011-02-28 22:04:38

--
-- PostgreSQL database dump complete
--

