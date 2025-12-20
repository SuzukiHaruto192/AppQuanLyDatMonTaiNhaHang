using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using doanlttq.MonAn;
using System.Windows;
using System.Diagnostics.Eventing.Reader;
using doanlttq.ViewModels;

namespace doanlttq.Services
{
    public class ICARS_ScoringService
    {
        private readonly int SCORE_WEATHER;
        private readonly int SCORE_TASTE;
        private readonly int SCORE_CROSS_SELL;
        private readonly int SCORE_SPECIAL;
        private readonly int SCORE_STRONG_DUPLICATE = 1000;

        public ICARS_ScoringService()
        {
            SCORE_WEATHER = Convert.ToInt32(ConfigurationManager.AppSettings["Score_Weather"]);
            SCORE_TASTE = Convert.ToInt32(ConfigurationManager.AppSettings["Score_Taste"]);
            SCORE_CROSS_SELL = Convert.ToInt32(ConfigurationManager.AppSettings["Score_CrossSell"]);
            SCORE_SPECIAL = Convert.ToInt32(ConfigurationManager.AppSettings["Score_Special"]);
        }

        public List<Food> CaculatorScore(List<Food> menu, string weatherContext, List<Food>? hoadon = null, List<string>? orderedTags = null)
        {
            foreach (Food dish in menu)
            {
                dish.score = 0;
                int totalScore = 0;

                if (dish.TagsMucDich.Contains("Đặc biệt") || dish.TagsMucDich.Contains("Bổ dưỡng"))
                {
                    totalScore += SCORE_SPECIAL;
                }

                //Cân bằng thời tiết
                if (weatherContext == "Lạnh")
                {
                    if (dish.TagsTinhChat.Contains("Nóng") || dish.TagsTinhChat.Contains("Thơm") || dish.TagsTinhChat.Contains("Béo") || dish.TagsTinhChat.Contains("Dịu"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                    else if (dish.TagsMucDich.Contains("Giữ ấm") || dish.TagsMucDich.Contains("Món chính") || dish.TagsMucDich.Contains("Bổ dưỡng"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                }

                if (weatherContext == "Mát")
                {
                    if (dish.TagsTinhChat.Contains("Nóng") || dish.TagsTinhChat.Contains("Thơm") || dish.TagsTinhChat.Contains("Thanh") || dish.TagsTinhChat.Contains("Dịu"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                    else if (dish.TagsMucDich.Contains("Khai vị") || dish.TagsMucDich.Contains("Món chính") || dish.TagsMucDich.Contains("Bổ dưỡng"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                }

                if (weatherContext == "Dễ chịu")
                {
                    if (dish.TagsTinhChat.Contains("Giòn") || dish.TagsTinhChat.Contains("Thơm") || dish.TagsTinhChat.Contains("Thanh") || dish.TagsTinhChat.Contains("Dịu"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                    else if (dish.TagsMucDich.Contains("Khai vị") || dish.TagsMucDich.Contains("Món chính") || dish.TagsMucDich.Contains("Ăn kèm"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                }

                if (weatherContext == "Nóng")
                {
                    if (dish.TagsTinhChat.Contains("Lạnh") || dish.TagsTinhChat.Contains("Chua") || dish.TagsTinhChat.Contains("Thanh") || dish.TagsTinhChat.Contains("Dịu") || dish.TagsTinhChat.Contains("Giòn"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                    else if (dish.TagsMucDich.Contains("Giải nhiệt") || dish.TagsMucDich.Contains("Giải khát") || dish.TagsMucDich.Contains("Tráng miệng") || dish.TagsMucDich.Contains("Rau xanh") || dish.TagsMucDich.Contains("Giải ngán"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                }

                if (weatherContext == "Rất nóng")
                {
                    if (dish.TagsTinhChat.Contains("Lạnh") || dish.TagsTinhChat.Contains("Chua") || dish.TagsTinhChat.Contains("Thanh"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                    else if (dish.TagsMucDich.Contains("Giải nhiệt") || dish.TagsMucDich.Contains("Giải khát") || dish.TagsMucDich.Contains("Tráng miệng") || dish.TagsMucDich.Contains("Rau xanh"))
                    {
                        totalScore += SCORE_WEATHER;
                    }
                }



                if (orderedTags != null && hoadon != null)
                {
                    // Cân bằng vị giác
                    if (orderedTags.Contains("Ngọt"))
                    {
                        if (dish.TagsTinhChat.Contains("Mặn") || dish.TagsTinhChat.Contains("Chua") || dish.TagsTinhChat.Contains("Đắng"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Mặn"))
                    {
                        if (dish.TagsTinhChat.Contains("Ngọt") || dish.TagsTinhChat.Contains("Thanh"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Béo"))
                    {
                        if (dish.TagsTinhChat.Contains("Cay") || dish.TagsTinhChat.Contains("Chua") || dish.TagsTinhChat.Contains("Thanh"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Cay"))
                    {
                        if (dish.TagsTinhChat.Contains("Ngọt") || dish.TagsTinhChat.Contains("Béo"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Chua"))
                    {
                        if (dish.TagsTinhChat.Contains("Ngọt") || dish.TagsTinhChat.Contains("Béo"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Đắng"))
                    {
                        if (dish.TagsTinhChat.Contains("Ngọt"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Nóng"))
                    {
                        if (dish.TagsTinhChat.Contains("Thanh") || dish.TagsTinhChat.Contains("Lạnh"))
                            totalScore += SCORE_TASTE;
                    }

                    if (orderedTags.Contains("Lạnh"))
                    {
                        if (dish.TagsTinhChat.Contains("Nóng") || dish.TagsTinhChat.Contains("Thơm"))
                            totalScore += SCORE_TASTE;
                    }


                    // Bán chéo
                    if (orderedTags.Contains("Món chính"))
                    {
                        if (dish.TagsMucDich.Contains("Ăn kèm") || dish.TagsMucDich.Contains("Phụ trợ") || dish.TagsMucDich.Contains("Tráng miệng"))
                        {
                            totalScore += SCORE_CROSS_SELL;
                        }
                    }
                    //Ngăn chặn món ăn trùng lặp
                    if (hoadon.Any(f => f.MAMON == dish.MAMON))
                        totalScore -= SCORE_STRONG_DUPLICATE;
                }
                dish.score = totalScore;
            }
            var filteredList = menu.OrderByDescending(d => d.score).ToList();

            return filteredList;

        }
    }
}
