/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */


/*************************************************************************************************************

PostMortem Database Create Script (PostGres with timescaledb)

CREATE EXTENSION timescaledb;

-- Database: surge

-- DROP DATABASE surge;

CREATE DATABASE surge
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'English_United States.1252'
    LC_CTYPE = 'English_United States.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

ALTER DATABASE surge
    SET search_path TO public, postgis, timescale;
		
CREATE SCHEMA postmortem
    AUTHORIZATION postgres;
	
-- DROP TABLE postmortem.instruction;

CREATE TABLE postmortem.instruction
(
    isid uuid NOT NULL,
    is_assigned timestamp with time zone NOT NULL,
    iid uuid NOT NULL,
    type character varying COLLATE pg_catalog."default",
    start timestamp with time zone NOT NULL,
    finish timestamp with time zone NOT NULL,
    stage character varying COLLATE pg_catalog."default",
    exceptions boolean
)
WITH(
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE postmortem.instruction
    OWNER to postgres;

SELECT create_hypertable('postmortem.instruction', 'start')

-- Index: instruction_is_assigned_idx

-- DROP INDEX postmortem.instruction_is_assigned_idx;

CREATE INDEX instruction_is_assigned_idx
    ON postmortem.instruction USING btree
    (is_assigned DESC)
    INCLUDE(isid, iid, type)
    TABLESPACE pg_default;

ALTER TABLE postmortem.instruction
    CLUSTER ON instruction_is_assigned_idx;

-- Trigger: ts_insert_blocker

-- DROP TRIGGER ts_insert_blocker ON postmortem.instruction;

CREATE TRIGGER ts_insert_blocker
    BEFORE INSERT
    ON postmortem.instruction
    FOR EACH ROW
    EXECUTE PROCEDURE _timescaledb_internal.insert_blocker();
	
	
	
-- DROP TABLE postmortem.instruction_set;

CREATE TABLE postmortem.instruction_set
(
    isid uuid NOT NULL,
    branch_ip character varying COLLATE pg_catalog."default",
    deployment_controller_id character varying COLLATE pg_catalog."default",
    deployment_manager_ip character varying COLLATE pg_catalog."default",
    deployment_controller character varying COLLATE pg_catalog."default",
    instruction_set_template character varying COLLATE pg_catalog."default",
    initiation_source character varying COLLATE pg_catalog."default",
    process_name character varying COLLATE pg_catalog."default",
    assigned timestamp with time zone NOT NULL,
    received timestamp with time zone NOT NULL,
    started timestamp with time zone NOT NULL,
    completed timestamp with time zone NOT NULL
)
WITH(
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE postmortem.instruction_set
    OWNER to postgres;
    
SELECT create_hypertable('postmortem.instruction_set', 'assigned')

-- Index: instruction_set_assigned_idx

-- DROP INDEX postmortem.instruction_set_assigned_idx;

CREATE INDEX instruction_set_assigned_idx
    ON postmortem.instruction_set USING btree
    (assigned DESC)
    INCLUDE(isid, branch_ip, deployment_controller, initiation_source, process_name)
    TABLESPACE pg_default;

ALTER TABLE postmortem.instruction_set
    CLUSTER ON instruction_set_assigned_idx;

-- Trigger: ts_insert_blocker

-- DROP TRIGGER ts_insert_blocker ON postmortem.instruction_set;

CREATE TRIGGER ts_insert_blocker
    BEFORE INSERT
    ON postmortem.instruction_set
    FOR EACH ROW
    EXECUTE PROCEDURE _timescaledb_internal.insert_blocker();

*************************************************************************************************************/


using System;
using System.Data;
using System.ComponentModel;

namespace STEM.Surge.PostGreSQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("ImportInstructionSetPostMortem")]
    [Description("Surge uses a PostGres database for PostMortem statistics. This instruction can be configured to " +
        "import a PostMortem into the Surge PostMortem database.")]

    public class ImportInstructionSetPostMortem : ImportPostMortem
    {
        [DisplayName("File Path"), DescriptionAttribute("The path of source file."), Category("Source")]
        public string Source { get; set; }

        public ImportInstructionSetPostMortem()
        {
            Source = "[TargetPath]\\[TargetName]";
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            Source = STEM.Sys.IO.Path.AdjustPath(Source);

            try
            {
                DataTable iSet = Build_ISetTable();
                DataTable instructions = Build_InstructionTable();
                IngestInstructionSet(System.IO.File.ReadAllText(Source), iSet, instructions);

                ImportDataTable(iSet, iSet.TableName);
                ImportDataTable(instructions, instructions.TableName);
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
