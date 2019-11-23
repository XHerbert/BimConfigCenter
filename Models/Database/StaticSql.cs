namespace IntegrateWebApp.Models.Database
{
    public class StaticSql
    {
        /// <summary>
        /// 项目列表
        /// </summary>
        public static string PROJECT_LIST = "SELECT ID,NAME,CODE FROM `t_project` WHERE ACTIVE_FLAG = 1 ORDER BY ID DESC;";

        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        public static string PROJECT = "SELECT ID Id,NAME Name,CODE Code,TPL_ID TplId FROM `t_project` WHERE ID = {0} AND ACTIVE_FLAG = 1;";

        /// <summary>
        /// 一级系统配置
        /// </summary>
        public static string SYSTEM_CONFIG = @"SELECT ID id,`SYS_NAME` sysName, `SYS_CODE` sysCode,`PROJECT_ID` projectId,`SYS_ICON` sysIcon,`SYS_ORDER` sysOrder ,
                                                `CREATOR` creator,`CREATE_TIME` createTime,`UPDATE_TIME` updateTime,`PARENT_CODE` parentCode,`IS_SHOW` isShow,`API` api,
                                                `IOT_API` iotApi,`UI_STYLE` uiStyle,`IOT_MODEL_CODE` iotModelCode,`SYS_PIC` sysPic,`MODEL_STYLE` modelStyle,`MODEL_SHOW_CONDITION` modelShowCondition,`SYS_COLOR` sysColor
                                                FROM `gfm_system_config` WHERE PROJECT_ID = {0};";

        /// <summary>
        /// 项目下模型文件列表
        /// </summary>
        public static string PROJECT_MODEL_LIST = @"SELECT u.M_ID FILEID,u.NAME,f.`FLOOR`,f.`SPE` SPECIALTY FROM `p_file_upload` u JOIN p_file f ON f.`ID`=u.`FILE_ID`
                                                    WHERE u.POSTFIX = 'rvt' AND u.FILE_ID IN(SELECT ID FROM p_file WHERE TASK_ID IN (SELECT ID FROM p_task WHERE PROJECT_ID = {0}))
                                                    ORDER BY u.CREATE_TIME DESC ;";

        /// <summary>
        /// 查询楼层信息
        /// </summary>
        public static string FLOORS = "SELECT ID,LINE_CODE FloorCode,LINE_NAME FloorName FROM `t_dictionary_lines` WHERE `DICTIONARY_ID`=1 AND ACTIVE_FLAG=1;";

        /// <summary>
        /// 查询专业信息
        /// </summary>
        public static string SPECIALTY = "SELECT ID,LINE_CODE SpecialtyCode,LINE_NAME SpecialtyName FROM `t_dictionary_lines` WHERE `DICTIONARY_ID`=2 AND ACTIVE_FLAG=1;";

        /// <summary>
        /// 查询场景配置
        /// </summary>
        public static string SCENES = @"SELECT SCENE_ID sceneId,`PROJECT_ID` projectId,`MERGE_NAME` mergeName,`MERGE_MEMO` mergeMemo,
                                        `FUNC_CODE` funcCode,`INTEGRATE_ID` integrateId,`MERGER` merger,
                                        `BACK_PIC` backPic,`SHOW_SET` showSet,`BORDER_LINE` borderLine,`SHOW_SUN` showSun,`CAMERA` camera,`DEF_API` defApi FROM `gfm_bim_appinfo` ";

        /// <summary>
        /// 获取数字化移交设备分类
        /// </summary>
        public static string TPL_FILE = "SELECT ID Id,`PROJECT_ID` ProjectId,`CODE` Code,`NAME` Name,`PARENT_ID` ParentId,SEQ Seq FROM `p_tpl_file` WHERE DIR_FLAG IN (1,2) AND TPL_ID = {0} AND FILE_TYPE = 5 AND PROJECT_ID = {1} order by NAME ASC;";

        /// <summary>
        /// 批量添加空间信息
        /// </summary>
        public static string ADD_SPACE_WEIGHT = "INSERT INTO  `gfm_energy_interpolation_weight` (`PROJECT_ID`,`INTEGRATION_ID`,`SPACE_ID`,`FLOOR`,`WEIGHT`,`CREATE_TIME`,`BATCH_GUID`) VALUES {0}";

        /// <summary>
        /// 更新权重信息
        /// </summary>
        public static string UPDATE_WEIGHT = "UPDATE `gfm_energy_interpolation_weight` SET WEIGHT = '{0}' WHERE SPACE_ID = '{1}'";

        public static string QUERY_SPACE = "SELECT SPACE_ID Id,WEIGHT Weight FROM `gfm_energy_interpolation_weight`";

        /// <summary>
        /// 温感（烟感代替）信息
        /// </summary>
        public static string TEMPERATURE_POINT = @"SELECT Id,BoundingBox FROM `bim_t_element` WHERE Id IN 
                                                (
                                                    '1689689599747552.2350281',
                                                    '1689689599747552.2353938',
                                                    '1689689599747552.2350956',
                                                    '1689689599747552.2350638',
                                                    '1689689599747552.2348600',
                                                    '1689689599747552.2351604',
                                                    '1689689599747552.2347245',
                                                    '1689689599747552.2351041',
                                                    '1689689599747552.2352443',
                                                    '1689689599747552.2346298',
                                                    '1689689599747552.2347599',
                                                    '1689689599747552.2361359',
                                                    '1689689599747552.2348737',
                                                    '1689689599747552.2349029',
                                                    '1689689599747552.2349119',
                                                    '1689689599747552.2349196',
                                                    '1689689599747552.2353105',
                                                    '1689689599747552.2353538',
                                                    '1689689599747552.2346327',
                                                    '1689689599747552.2346485',
                                                    '1689689599747552.2346512'
                                                  )";
    }
}