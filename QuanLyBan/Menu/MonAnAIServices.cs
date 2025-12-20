using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Chat;
using Newtonsoft.Json;

namespace QuanLyBan.Menu
{
    public class MonAnAIServices
    {
        private readonly string api_key = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
        private ChatClient chatClient;
        public MonAnAIServices()
        {
            chatClient = new ChatClient("gpt-4o-mini", api_key);
        }

        public async Task<MonAnAI> GetTags(string TenMon)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    @"
        Bạn là AI chuyên phân tích món ăn Việt Nam.
        Chỉ trả về JSON đúng mẫu. 
        
        Quy tắc quan trọng:
        1. Các tag phải là một chuỗi văn bản (String).
        2. Các từ khoá trong chuỗi PHẢI được ngăn cách nhau bởi dấu phẩy và một khoảng trắng (, ).
        3. KHÔNG tạo tag mới ngoài danh sách cho phép
        4. ""Giải khát"" CHỈ dùng cho đồ uống, KHÔNG dùng cho món ăn
        5. TagTinhChat chỉ được dùng các giá trị: Nóng, Thơm, Dịu, Mặn, Ngọt, Thanh, Béo, Lạnh, Chua, Cay, Giòn, Đắng, Dẻo
        6. TagMucDich chỉ được dùng các giá trị: Giữ ấm, Khai vị, Giải nhiệt, Ăn kèm, Món chính, Ăn vặt, Đặc biệt, Bổ dưỡng, Hải sản, Tráng miệng, Giữ ấm, Giải khát, Phụ Trợ, Rau xanh, Giải ngán

        Ví dụ mẫu output mong muốn:
        {
            ""TenMon"": ""Tên món"",
            ""MoTa"": ""Mô tả ngắn gọn nhất về món ăn"",
            ""TagTinhChat"": ""Cay, Nóng, Mặn"", 
            ""TagMucDich"": ""Giữ ấm, Khai vị, Giải nhiệt, Ăn kèm, Món chính, Giải khát""
        }
        
        Tuyệt đối không giải thích thêm.
    "
                    ),

                new UserChatMessage($"Phân tích món: {TenMon}")
            };

            var response = await chatClient.CompleteChatAsync(messages);
            string json = response.Value.Content[0].Text;

            return JsonConvert.DeserializeObject<MonAnAI>(json);
        }
    }
}
