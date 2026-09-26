CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Enclosures" (
    "Id" uuid NOT NULL,
    "Name" text,
    "Area" double precision NOT NULL,
    "Capacity" integer NOT NULL,
    CONSTRAINT "PK_Enclosures" PRIMARY KEY ("Id")
);

CREATE TABLE "Animals" (
    "Id" uuid NOT NULL,
    "Name" text,
    "Species" text,
    "Age" integer NOT NULL,
    "EnclosureId" uuid,
    CONSTRAINT "PK_Animals" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Animals_Enclosures_EnclosureId" FOREIGN KEY ("EnclosureId") REFERENCES "Enclosures" ("Id") ON DELETE SET NULL
);

CREATE TABLE "AnimalDetails" (
    "AnimalId" uuid NOT NULL,
    "MedicalNotes" text,
    CONSTRAINT "PK_AnimalDetails" PRIMARY KEY ("AnimalId"),
    CONSTRAINT "FK_AnimalDetails_Animals_AnimalId" FOREIGN KEY ("AnimalId") REFERENCES "Animals" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Birds" (
    "Id" uuid NOT NULL,
    "Wingspan" double precision NOT NULL,
    "CanFly" boolean NOT NULL,
    "FeatherColor" text,
    CONSTRAINT "PK_Birds" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Birds_Animals_Id" FOREIGN KEY ("Id") REFERENCES "Animals" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Mammals" (
    "Id" uuid NOT NULL,
    "FurColor" text,
    "Diet" text,
    "Weight" double precision NOT NULL,
    CONSTRAINT "PK_Mammals" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Mammals_Animals_Id" FOREIGN KEY ("Id") REFERENCES "Animals" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Animals_EnclosureId" ON "Animals" ("EnclosureId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260910140909_InitialCreate', '8.0.11');

COMMIT;

