using System;

namespace ConfigCenterApp.Models.Entity
{
    public class SystemConfig
    {
        public int id { get; set; }

        public string sysName { get; set; }

        public string sysCode { get; set; }

        public int projectId { get; set; }

        public string sysIcon { get; set; }

        public int sysOrder { get; set; }

        public string creator { get; set; }

        public DateTime? createTime { get; set; }

        public DateTime? updateTime { get; set; }

        public string parentCode { get; set; }

        public int isShow { get; set; }

        public string api { get; set; }

        public string iotApi { get; set; }

        public string uiStyle { get; set; }

        public string iotModelCode { get; set; }

        /**
         * 系统级参数配置中展示的图片
         */
        public string sysPic { get; set; }

        /**
         * 0: 仅着色   1:着色且带背景颜色的一级系统图标
         */
        public int modelStyle { get; set; }

        public string modelShowCondition { get; set; }

        /**
         * 仅用在报警数据统计颜色
         */
        public string sysColor { get; set; }
    }
}