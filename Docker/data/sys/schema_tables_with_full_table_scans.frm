TYPE=VIEW
query=select `performance_schema`.`table_io_waits_summary_by_index_usage`.`OBJECT_SCHEMA` AS `object_schema`,`performance_schema`.`table_io_waits_summary_by_index_usage`.`OBJECT_NAME` AS `object_name`,`performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` AS `rows_full_scanned`,format_pico_time(`performance_schema`.`table_io_waits_summary_by_index_usage`.`SUM_TIMER_WAIT`) AS `latency` from `performance_schema`.`table_io_waits_summary_by_index_usage` where `performance_schema`.`table_io_waits_summary_by_index_usage`.`INDEX_NAME` is null and `performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` > 0 order by `performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` desc
md5=dcd97456a8df9123b94972666be066a5
updatable=1
algorithm=1
definer_user=mariadb.sys
definer_host=localhost
suid=0
with_check_option=0
<<<<<<< HEAD
timestamp=0001718199721063832
=======
timestamp=0001718271630936300
>>>>>>> 381248087d981d82af2c3917e19a55ccd8764ee0
create-version=2
source=SELECT object_schema,\n       object_name,\n       count_read AS rows_full_scanned,\n       format_pico_time(sum_timer_wait) AS latency\n  FROM performance_schema.table_io_waits_summary_by_index_usage\n WHERE index_name IS NULL\n   AND count_read > 0\n ORDER BY count_read DESC;
client_cs_name=utf8mb3
connection_cl_name=utf8mb3_general_ci
view_body_utf8=select `performance_schema`.`table_io_waits_summary_by_index_usage`.`OBJECT_SCHEMA` AS `object_schema`,`performance_schema`.`table_io_waits_summary_by_index_usage`.`OBJECT_NAME` AS `object_name`,`performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` AS `rows_full_scanned`,format_pico_time(`performance_schema`.`table_io_waits_summary_by_index_usage`.`SUM_TIMER_WAIT`) AS `latency` from `performance_schema`.`table_io_waits_summary_by_index_usage` where `performance_schema`.`table_io_waits_summary_by_index_usage`.`INDEX_NAME` is null and `performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` > 0 order by `performance_schema`.`table_io_waits_summary_by_index_usage`.`COUNT_READ` desc
mariadb-version=110402
