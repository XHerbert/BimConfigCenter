using ConfigCenterApp.Models.Entity;
using IntegrateWebApp.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ConfigCenterApp.Controllers
{
    public class WeightController : Controller
    {

        private readonly int m_useWeightCount = 21;
        private readonly double m_usePower = 2;
        private Dictionary<string, List<TemperatureParameter>> m_tempMap = new Dictionary<string, List<TemperatureParameter>>();
        private Dictionary<string, List<BlockParameter>> m_blockMap = new Dictionary<string, List<BlockParameter>>();
        private Dictionary<string, double> m_temperatureValue = new Dictionary<string, double>();
        private Dictionary<string, string> finalData = new Dictionary<string, string>();

        // GET: Weight
        public ActionResult Index(bool write = false, string viewToken = "003b95d50b554031a213ff9eb3a5183a")
        {
            InitTemperatureValue2();
            List<double> values = new List<double>();
            List<TemperaturePoint> temperaturePoints = new List<TemperaturePoint>();
            string sql = StaticSql.TEMPERATURE_POINT;
            string querySpace = StaticSql.QUERY_SPACE;
            using (var db = new IntegrateDbContext())
            {
                List<TemperaturePointOriginal> temperaturePointOriginals = db.Database.SqlQuery<TemperaturePointOriginal>(sql).ToList();
                temperaturePointOriginals.ForEach(item =>
                {
                    BoundingBox boundingbox = JsonConvert.DeserializeObject<BoundingBox>(item.BoundingBox);
                    double x = (boundingbox.max.x + boundingbox.min.x) * 0.5;
                    double y = (boundingbox.max.y + boundingbox.min.y) * 0.5;
                    double z = (boundingbox.max.z + boundingbox.min.z) * 0.5;
                    var position = string.Format("{0},{1},{2}", x, y, z);
                    temperaturePoints.Add(new TemperaturePoint { objectId = item.Id, position = position });
                });

                List<PointWeight> pointWeights = db.Database.SqlQuery<PointWeight>(querySpace).ToList();
                int R = 0; int G = 0; int B = 0;
                pointWeights.ForEach(item =>
                {
                    //if (string.IsNullOrEmpty(item.Weight))
                    //    return;
                    // 1689689599747552.2346327:0.0961006025123045; 1689689599747552.2346298:0.0901430435338005; 
                    double degree = 0;
                    string[] weights = item.Weight.Split(';');
                    foreach (string weight in weights)
                    {
                        string[] keyValue = weight.Split(':');
                        string key = (keyValue[0]);
                        double value = Convert.ToDouble(keyValue[1]);
                        if (!m_temperatureValue.ContainsKey(keyValue[0]))
                            continue;
                        degree += m_temperatureValue[keyValue[0]] * value;
                    }
                    Color realrgb = ConvertDegreeToRGB(degree);
                    values.Add(degree); 
                    R = realrgb.R; G = realrgb.G; B = realrgb.B;
                    string rgb = string.Format("{0},{1},{2}", R, G, B);
                    //直接把当前元素添加到，key对应的List集合中。
                    finalData[item.Id] = rgb;
                });
                ViewBag.TemperatureData = JsonConvert.SerializeObject(finalData.ToList());
                ViewBag.V = JsonConvert.SerializeObject(values);
            }
            ViewBag.viewToken = viewToken;
            return View();
        }

        public ActionResult GenerateData(bool write = true)
        {
            PrepareBlock("F01");
            PrepareTemperature("F01");
            ComputeAllWeight();
            if (write)
            {
                OutputWeight();
            }
            return Content("ok");
        }


        /// <summary>
        /// 准备空间数据，空间Guid和position位置信息
        /// </summary>
        private void PrepareBlock(string floor)
        {
            List<BlockPoint> items = new List<BlockPoint>();
            using (StreamReader reader = new StreamReader(Server.MapPath("~/App_Data/data.json")))
            {
                string json = reader.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<BlockPoint>>(json);
            }

            var guid = Guid.NewGuid().ToString();
            int projectId = 366;
            long integration = 1736233257547680;
            StringBuilder builder = new StringBuilder();
            foreach (BlockPoint dr in items)
            {
                List<BlockParameter> blockPointsParameters = null;
                if (m_blockMap.ContainsKey(floor))
                {
                    blockPointsParameters = m_blockMap[floor];
                }
                else
                {
                    blockPointsParameters = new List<BlockParameter>();
                    m_blockMap.Add(floor, blockPointsParameters);
                }
                BlockParameter blockPara = new BlockParameter();
                string[] xyz = dr.position.Split(',');
                blockPara.px = Convert.ToDouble(xyz[0]);
                blockPara.py = Convert.ToDouble(xyz[1]);
                blockPara.pz = Convert.ToDouble(xyz[2]);
                blockPara.id = dr.id;
                blockPara.relationIdWeight = new Dictionary<string, double>();
                blockPointsParameters.Add(blockPara);

                builder.AppendFormat("({0},{1},'{2}','{3}','','{4}','{5}'),", projectId, integration, dr.id, floor, DateTime.Now, guid);
            }
            var sqlValue = builder.ToString().TrimEnd(',');
            var sql = string.Format(StaticSql.ADD_SPACE_WEIGHT, sqlValue);
            using (var db = new IntegrateDbContext())
            {
                int ret = db.Database.ExecuteSqlCommand(sql);
                Console.WriteLine(ret);
            }
        }

        /// <summary>
        /// 准备温感点位数据，Id和position位置信息
        /// </summary>
        /// <param name="floorName"></param>
        private void PrepareTemperature(string floorName)
        {
            List<TemperaturePoint> temperaturePoints = new List<TemperaturePoint>();
            string sql = StaticSql.TEMPERATURE_POINT;
            using (var db = new IntegrateDbContext())
            {
                List<TemperaturePointOriginal> temperaturePointOriginals = db.Database.SqlQuery<TemperaturePointOriginal>(sql).ToList();
                temperaturePointOriginals.ForEach(item =>
                {
                    BoundingBox boundingbox = JsonConvert.DeserializeObject<BoundingBox>(item.BoundingBox);
                    double x = (boundingbox.max.x + boundingbox.min.x) * 0.5;
                    double y = (boundingbox.max.y + boundingbox.min.y) * 0.5;
                    double z = (boundingbox.max.z + boundingbox.min.z) * 0.5;
                    var position = string.Format("{0},{1},{2}", x, y, z);
                    temperaturePoints.Add(new TemperaturePoint { objectId = item.Id, position = position });
                });
            }

            foreach (TemperaturePoint dr in temperaturePoints)
            {
                List<TemperatureParameter> temperaturePointsParameters = null;
                if (m_tempMap.ContainsKey(floorName))
                {
                    temperaturePointsParameters = m_tempMap[floorName];
                }
                else
                {
                    temperaturePointsParameters = new List<TemperatureParameter>();
                    m_tempMap.Add(floorName, temperaturePointsParameters);
                }

                TemperatureParameter tempPara = new TemperatureParameter();
                string[] xyz = dr.position.Split(',');
                tempPara.px = Convert.ToDouble(xyz[0]);
                tempPara.py = Convert.ToDouble(xyz[1]);
                tempPara.pz = Convert.ToDouble(xyz[2]);
                tempPara.id = dr.objectId;
                temperaturePointsParameters.Add(tempPara);
            }
        }

        /// <summary>
        /// 计算权重
        /// </summary>
        private void ComputeAllWeight()
        {
            foreach (KeyValuePair<string, List<BlockParameter>> kvp in m_blockMap)
            {
                List<TemperatureParameter> temperaturePointsParameters = m_tempMap[kvp.Key];
                int count = kvp.Value.Count;
                for (int i = 0; i < count; i++)
                {
                    Dictionary<string, double> relationIdWeight = ComputeSingleWeight(kvp.Value[i], temperaturePointsParameters);
                    foreach (KeyValuePair<string, double> kvpInteral in relationIdWeight)
                    {
                        kvp.Value[i].relationIdWeight.Add(kvpInteral.Key, kvpInteral.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 计算权重的核心算法
        /// </summary>
        /// <param name="blockPara">每一个体量空间信息</param>
        /// <param name="temperaturePointsParameters">14个温度探测器</param>
        /// <returns></returns>
        private Dictionary<string, double> ComputeSingleWeight(BlockParameter blockPara, List<TemperatureParameter> temperaturePointsParameters)
        {
            double x = blockPara.px; double y = blockPara.py; double z = blockPara.pz;
            Dictionary<string, double> relationIdWeight = new Dictionary<string, double>();

            Dictionary<string, double> distArray = new Dictionary<string, double>();

            foreach (TemperatureParameter tempPara in temperaturePointsParameters)
            {
                double tempx = tempPara.px;
                double tempy = tempPara.py;
                double tempz = tempPara.pz;
                double dist = Math.Sqrt((x - tempx) * (x - tempx) + (y - tempy) * (y - tempy) + (z - tempz) * (z - tempz));
                if (dist < 1e-6)
                {
                    //数值特别小，距离特别短，几乎重合的时候，直接设置权重为1
                    relationIdWeight.Add(tempPara.id, 1);
                    return relationIdWeight;
                }

                //对dist进行乘方运算
                dist = Math.Pow(dist, m_usePower);
                //对dist取倒数，数值越小，距离越远，权重越低
                distArray.Add(tempPara.id, 1 / dist);
            }

            var dicSort = from objDic in distArray orderby objDic.Value descending select objDic;
            int count = 1;
            //计算出dist总和
            double sum = 0.0;
            foreach (KeyValuePair<string, double> kvp in dicSort)
            {
                sum += kvp.Value;
                count++;
                if (count > m_useWeightCount)
                    break;
            }

            count = 1;
            foreach (KeyValuePair<string, double> kvp in dicSort)
            {
                //计算每个值占总和的比例
                relationIdWeight.Add(kvp.Key, kvp.Value / sum);
                count++;
                if (count > m_useWeightCount)
                    break;
            }
            return relationIdWeight;
        }

        /// <summary>
        /// 更新权重信息
        /// </summary>
        private void OutputWeight()
        {
            using (var db = new IntegrateDbContext())
            {
                foreach (List<BlockParameter> blockPointsParameters in m_blockMap.Values)
                {
                    foreach (BlockParameter blockPara in blockPointsParameters)
                    {
                        var id = blockPara.id.ToString();
                        var weights = string.Empty;
                        foreach (KeyValuePair<string, double> kvp in blockPara.relationIdWeight)
                        {
                            weights += kvp.Key.ToString() + ":" + kvp.Value.ToString() + ";";
                        }
                        weights = weights.TrimEnd(';');
                        var ret = db.Database.ExecuteSqlCommand(string.Format(StaticSql.UPDATE_WEIGHT, weights, id));
                    }
                }
            }
        }

        /// <summary>
        /// 颜色装换
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        private Color ConvertDegreeToRGB(double degree)
        {
            byte realr = 0;
            byte realg = 0;
            byte realb = 0;

            if (degree < 15)
            {
                realr = 0;
                realg = 109;
                realb = 192;
            }
            else if (degree < 16)
            {
                double degreediff = degree - 15;
                double rdiff = 0 - 0;
                double gdiff = 192 - 109;
                double bdiff = 255 - 192;
                realr = (byte)(0 + degreediff / 1 * rdiff);
                realg = (byte)(109 + degreediff / 1 * gdiff);
                realb = (byte)(192 + degreediff / 1 * bdiff);
            }
            else if (degree < 17)
            {
                double degreediff = degree - 16;
                double rdiff = 0 - 0;
                double gdiff = 246 - 192;
                double bdiff = 255 - 255;
                realr = (byte)(0 + degreediff / 1 * rdiff);
                realg = (byte)(192 + degreediff / 1 * gdiff);
                realb = (byte)(255 + degreediff / 1 * bdiff);
            }
            else if (degree < 20)
            {
                double degreediff = degree - 17;
                double rdiff = 100 - 0;
                double gdiff = 255 - 246;
                double bdiff = 79 - 255;
                realr = (byte)(0 + degreediff / 3 * rdiff);
                realg = (byte)(246 + degreediff / 3 * gdiff);
                realb = (byte)(255 + degreediff / 3 * bdiff);
            }
            else if (degree < 22)
            {
                double degreediff = degree - 20;
                double rdiff = 26 - 100;
                double gdiff = 231 - 255;
                double bdiff = 0 - 79;
                realr = (byte)(100 + degreediff / 2 * rdiff);
                realg = (byte)(255 + degreediff / 2 * gdiff);
                realb = (byte)(79 + degreediff / 2 * bdiff);
            }
            else if (degree < 24)
            {
                double degreediff = degree - 22;
                double rdiff = 20 - 26;
                double gdiff = 177 - 231;
                double bdiff = 0 - 0;
                realr = (byte)(26 + degreediff / 2 * rdiff);
                realg = (byte)(231 + degreediff / 2 * gdiff);
                realb = (byte)(0 + degreediff / 2 * bdiff);
            }
            else if (degree < 27)
            {
                double degreediff = degree - 24;
                double rdiff = 249 - 20;
                double gdiff = 226 - 177;
                double bdiff = 24 - 0;
                realr = (byte)(20 + degreediff / 3 * rdiff);
                realg = (byte)(177 + degreediff / 3 * gdiff);
                realb = (byte)(0 + degreediff / 3 * bdiff);
            }
            else if (degree < 28)
            {
                double degreediff = degree - 27;
                double rdiff = 188 - 249;
                double gdiff = 2 - 226;
                double bdiff = 111 - 24;
                realr = (byte)(249 + degreediff / 1 * rdiff);
                realg = (byte)(226 + degreediff / 1 * gdiff);
                realb = (byte)(24 + degreediff / 1 * bdiff);
            }
            else if (degree < 29)
            {
                double degreediff = degree - 28;
                double rdiff = 208 - 188;
                double gdiff = 0 - 2;
                double bdiff = 32 - 111;
                realr = (byte)(188 + degreediff / 1 * rdiff);
                realg = (byte)(2 + degreediff / 1 * gdiff);
                realb = (byte)(111 + degreediff / 1 * bdiff);
            }
            else
            {
                realr = 208;
                realg = 0;
                realb = 32;
            }
            return Color.FromArgb(realr, realg, realb);
        }

        /// <summary>
        /// 模拟温感数据
        /// </summary>
        private void InitTemperatureValue()
        {
            m_temperatureValue.Clear();
            m_temperatureValue.Add("1689689599747552.2350281", 12.1);
            m_temperatureValue.Add("1689689599747552.2353938", 34.3);
            m_temperatureValue.Add("1689689599747552.2350956", 16.3);
            m_temperatureValue.Add("1689689599747552.2350638", 85.6);
            m_temperatureValue.Add("1689689599747552.2348600", 12.0);
            m_temperatureValue.Add("1689689599747552.2351604", 19.0);
            m_temperatureValue.Add("1689689599747552.2347245", 22.9);
            m_temperatureValue.Add("1689689599747552.2351041", 21.6);
            m_temperatureValue.Add("1689689599747552.2352443", 27.3);
            m_temperatureValue.Add("1689689599747552.2346298", 38.5);
            m_temperatureValue.Add("1689689599747552.2347599", 35.6);
            m_temperatureValue.Add("1689689599747552.2361359", 30.1);
            m_temperatureValue.Add("1689689599747552.2348737", 3.0);
            m_temperatureValue.Add("1689689599747552.2349029", 29.5);
            m_temperatureValue.Add("1689689599747552.2349119", 44.6);
            m_temperatureValue.Add("1689689599747552.2349196", 22.2);
            m_temperatureValue.Add("1689689599747552.2353105", 53.3);
            m_temperatureValue.Add("1689689599747552.2353538", 32.2);
            m_temperatureValue.Add("1689689599747552.2346327", 32.1);
            m_temperatureValue.Add("1689689599747552.2346485", 20.0);
            m_temperatureValue.Add("1689689599747552.2346512", 66.4);

            //m_temperatureValue.Add("1689689599747552.2350281", 10.0);
            //m_temperatureValue.Add("1689689599747552.2353938", 20.0);
            //m_temperatureValue.Add("1689689599747552.2350956", 30.0);
            //m_temperatureValue.Add("1689689599747552.2350638", 40.0);
            //m_temperatureValue.Add("1689689599747552.2348600", 30.0);
            //m_temperatureValue.Add("1689689599747552.2351604", 20.0);
            //m_temperatureValue.Add("1689689599747552.2347245", 10.0);
            //m_temperatureValue.Add("1689689599747552.2351041", 20.0);
            //m_temperatureValue.Add("1689689599747552.2352443", 30.0);
            //m_temperatureValue.Add("1689689599747552.2346298", 40.0);
            //m_temperatureValue.Add("1689689599747552.2347599", 50.0);
            //m_temperatureValue.Add("1689689599747552.2361359", 60.0);
            //m_temperatureValue.Add("1689689599747552.2348737", 50.0);
            //m_temperatureValue.Add("1689689599747552.2349029", 40.0);
            //m_temperatureValue.Add("1689689599747552.2349119", 30.0);
            //m_temperatureValue.Add("1689689599747552.2349196", 20.0);
            //m_temperatureValue.Add("1689689599747552.2353105", 10.0);
            //m_temperatureValue.Add("1689689599747552.2353538", 60.0);
            //m_temperatureValue.Add("1689689599747552.2346327", 20.0);
            //m_temperatureValue.Add("1689689599747552.2346485", 50.0);
            //m_temperatureValue.Add("1689689599747552.2346512", 10.0);
        }

        private void InitTemperatureValue2()
        {
            m_temperatureValue.Clear();
            m_temperatureValue.Add("1689689599747552.2350281", 10.0);
            m_temperatureValue.Add("1689689599747552.2353938", 10.0);
            m_temperatureValue.Add("1689689599747552.2350956", 10.0);
            m_temperatureValue.Add("1689689599747552.2350638", 10.0);
            m_temperatureValue.Add("1689689599747552.2348600", 10.0);
            m_temperatureValue.Add("1689689599747552.2351604", 10.0);
            m_temperatureValue.Add("1689689599747552.2347245", 10.0);
            m_temperatureValue.Add("1689689599747552.2351041", 10.0);
            m_temperatureValue.Add("1689689599747552.2352443", 10.0);
            m_temperatureValue.Add("1689689599747552.2346298", 50.0);
            m_temperatureValue.Add("1689689599747552.2347599", 50.0);
            m_temperatureValue.Add("1689689599747552.2361359", 50.0);
            m_temperatureValue.Add("1689689599747552.2348737", 50.0);
            m_temperatureValue.Add("1689689599747552.2349029", 50.0);
            m_temperatureValue.Add("1689689599747552.2349119", 10.0);
            m_temperatureValue.Add("1689689599747552.2349196", 10.0);
            m_temperatureValue.Add("1689689599747552.2353105", 10.0);
            m_temperatureValue.Add("1689689599747552.2353538", 10.0);
            m_temperatureValue.Add("1689689599747552.2346327", 10.0);
            m_temperatureValue.Add("1689689599747552.2346485", 10.0);
            m_temperatureValue.Add("1689689599747552.2346512", 10.0);

            //m_temperatureValue.Add("1689689599747552.2350281", 10.0);
            //m_temperatureValue.Add("1689689599747552.2353938", 20.0);
            //m_temperatureValue.Add("1689689599747552.2350956", 30.0);
            //m_temperatureValue.Add("1689689599747552.2350638", 40.0);
            //m_temperatureValue.Add("1689689599747552.2348600", 30.0);
            //m_temperatureValue.Add("1689689599747552.2351604", 20.0);
            //m_temperatureValue.Add("1689689599747552.2347245", 10.0);
            //m_temperatureValue.Add("1689689599747552.2351041", 20.0);
            //m_temperatureValue.Add("1689689599747552.2352443", 30.0);
            //m_temperatureValue.Add("1689689599747552.2346298", 40.0);
            //m_temperatureValue.Add("1689689599747552.2347599", 50.0);
            //m_temperatureValue.Add("1689689599747552.2361359", 60.0);
            //m_temperatureValue.Add("1689689599747552.2348737", 50.0);
            //m_temperatureValue.Add("1689689599747552.2349029", 40.0);
            //m_temperatureValue.Add("1689689599747552.2349119", 30.0);
            //m_temperatureValue.Add("1689689599747552.2349196", 20.0);
            //m_temperatureValue.Add("1689689599747552.2353105", 10.0);
            //m_temperatureValue.Add("1689689599747552.2353538", 60.0);
            //m_temperatureValue.Add("1689689599747552.2346327", 20.0);
            //m_temperatureValue.Add("1689689599747552.2346485", 50.0);
            //m_temperatureValue.Add("1689689599747552.2346512", 10.0);
        }

        struct BlockParameter
        {
            public double px;
            public double py;
            public double pz;
            public string id;
            public Dictionary<string, double> relationIdWeight;
        }

        struct TemperatureParameter
        {
            public double px;
            public double py;
            public double pz;
            public string id;
        }
    }
}