using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrateWebApp.Models.Database
{
    public class StaticSql
    {
        /// <summary>
        /// 项目列表
        /// </summary>
        public static string PROJECT_LIST = "SELECT ID,NAME,CODE FROM `t_project` WHERE ACTIVE_FLAG = 1 ORDER BY ID DESC;";

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

    }
}