TYPE=VIEW
query=select `sys`.`format_bytes`(sum(`performance_schema`.`memory_summary_global_by_event_name`.`CURRENT_NUMBER_OF_BYTES_USED`)) AS `total_allocated` from `performance_schema`.`memory_summary_global_by_event_name`
md5=8082fddb38d6165c0d33b88815ddf3d8
updatable=0
algorithm=2
definer_user=mariadb.sys
definer_host=localhost
suid=0
with_check_option=0
<<<<<<< HEAD
timestamp=0001718199720963139
=======
timestamp=0001718271630752360
>>>>>>> 381248087d981d82af2c3917e19a55ccd8764ee0
create-version=2
source=SELECT sys.format_bytes(SUM(CURRENT_NUMBER_OF_BYTES_USED)) total_allocated\n  FROM performance_schema.memory_summary_global_by_event_name;
client_cs_name=utf8mb3
connection_cl_name=utf8mb3_general_ci
view_body_utf8=select `sys`.`format_bytes`(sum(`performance_schema`.`memory_summary_global_by_event_name`.`CURRENT_NUMBER_OF_BYTES_USED`)) AS `total_allocated` from `performance_schema`.`memory_summary_global_by_event_name`
mariadb-version=110402
