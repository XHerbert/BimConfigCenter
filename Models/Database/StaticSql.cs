﻿using System;
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
        public static string SCENES = @"SELECT SCENE_ID Id,`PROJECT_ID` ProjectId,`MERGE_NAME` MergeName,`MERGE_MEMO` MergeMemo,
                                        `FUNC_CODE` FunctionCode,`INTEGRATE_ID` IntegrateId,`MERGE_TIME` MergeTime,`MERGER` Merger,
                                        `BACK_PIC` BackPicture,`SHOW_SET` ShowSet,`BORDER_LINE` BorderLine,`SHOW_SUN` ShowSun,`CAMERA` Camera,`DEF_API` DefineAPI FROM `gfm_bim_appinfo` WHERE `PROJECT_ID`={0} ";

    }
}