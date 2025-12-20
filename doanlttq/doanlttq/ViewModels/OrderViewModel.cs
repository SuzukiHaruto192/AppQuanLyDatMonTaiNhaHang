using doanlttq.MonAn;
using doanlttq.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace doanlttq.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        private readonly ICARS_ScoringService iCARS_ScoringService;
        private readonly WeatherService weatherService;

        public ObservableCollection<Food> GioHang { get; set; } = new ObservableCollection<Food>();
        public List<Food> hoadon { get; set; } = new List<Food>();

        private List<Food> _masterMenuList = new List<Food>();
        private ObservableCollection<Food> _foods;
        public ObservableCollection<Food> Foods
        {
            get
            {
                return _foods;
            }

            set
            {
                if (_foods != value)
                {
                    _foods = value;
                    OnPropertyChanged();
                }
            }
        }

        private DatabaseHelper db = new DatabaseHelper();

        public string LoaiMon { get; set; } = "L03";
        public List<string> CategoryId_Child = new List<string>
        {
            "L12", "L13", "L14", "L15"
        };

        public OrderViewModel(ICARS_ScoringService iCARS_ScoringService, WeatherService weatherService)
        {
            this.iCARS_ScoringService = iCARS_ScoringService;
            this.weatherService = weatherService;

            // ConfirmOrderCommand = new AsyncCommand(ExecuteConfirmAsync);

        }


        // Lọc món
        public void FilterMenu()
        {
            if (_masterMenuList == null || _masterMenuList.Count == 0)
                return;

            var ds_da_loc = _masterMenuList
                   .Where(f => CategoryId_Child.Contains(f.CATEGORYID))
                   .OrderByDescending(f => f.score)
                   .ToList();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Foods = new ObservableCollection<Food>(ds_da_loc);
            });


            //System.Diagnostics.Debug.WriteLine("------------------------------------------");
            //System.Diagnostics.Debug.WriteLine($"LOG LÚC: {DateTime.Now.ToString("HH:mm:ss")}");
            //System.Diagnostics.Debug.WriteLine("DANH SÁCH ĐIỂM SỐ SAU KHI TÍNH:");

            //foreach (var item in Foods)
            //{
            //    // In ra tên và điểm để kiểm tra
            //    System.Diagnostics.Debug.WriteLine($"Món: {item.TENMON} | Điểm: {item.score} ");
            //}
            //System.Diagnostics.Debug.WriteLine("------------------------------------------");
        }


        //Thêm vào giỏ hàng
        public void ThemGioHang(Food food)
        {
            var tonTai = GioHang.FirstOrDefault(x => x.MAMON == food.MAMON);

            if (tonTai == null)
            {
                var newItem = new Food
                {
                    MAMON = food.MAMON,
                    TENMON = food.TENMON,
                    GIA = food.GIA,
                    ANH = food.ANH,
                    MOTA = food.MOTA,
                    TagsTinhChat = food.TagsTinhChat,
                    TagsMucDich = food.TagsMucDich,
                    CATEGORYID = food.CATEGORYID,
                    SoLuong = 1
                };
                GioHang.Add(newItem);
            }
            else
            {
                tonTai.SoLuong++;
            }
        }

        //Tìm kiếm món ăn
        public void Tim_Kiem_Mon_An(string TenMon)
        {
            var kq = db.TimMon(TenMon, LoaiMon);

            Foods = new ObservableCollection<Food>(kq);
        }

        // Xác nhận
        public async Task ExecuteConfirmAsync()
        {
            if (GioHang == null || GioHang.Count == 0)
                return;
            foreach (var food in GioHang)
            {
                ((App)Application.Current).TongTien += food.GIA;
            }

            if (((App)Application.Current).ThemHDFirst == true)
            {
                db.ThemHoaDon(((App)Application.Current).TongTien);
                ((App)Application.Current).ThemHDFirst = false;
            }
            else
            {
                db.UpdateHoaDon(((App)Application.Current).TongTien, null, "Chưa thanh toán");
            }
            foreach (var food in GioHang)
            {
                db.ThemCTHD(food);
                var MonDaCo = hoadon.FirstOrDefault(x => x.MAMON == food.MAMON);

                if (MonDaCo != null)
                {
                    MonDaCo.SoLuong += food.SoLuong;
                }
                else
                {
                    hoadon.Add(food);
                }
            }
            var OrderedTags = TronTag();
            await UpdateRecommendations(OrderedTags);
            GioHang.Clear();

        }

        // Giảm số lượng món
        public void Giam_So_Luong_Mon(Food food)
        {
            food.SoLuong--;
            if (food.SoLuong == 0)
            {
                GioHang.Remove(food);
            }
        }

        //Xóa giỏ
        public void Xoa_Gio_Hang()
        {
            GioHang.Clear();
        }

        // Trộn Tags
        public List<string> TronTag()
        {
            var TagGioHang = GioHang.SelectMany(f => f.TagsTinhChat.Concat(f.TagsMucDich)).Distinct();
            var TagHoaDon = hoadon.SelectMany(f => f.TagsTinhChat.Concat(f.TagsMucDich)).Distinct();
            var OrderedTags = TagGioHang.Concat(TagHoaDon).ToList();
            return OrderedTags;
        }
        public async Task LoadRecommendations()
        {
            string dataWeatherRaw = await weatherService.FetchRawWeatherDataAsync();
            string weatherContext = weatherService.NormalizeWeatherContext(dataWeatherRaw);
            List<Food> menu = await Task.Run(() => db.GetFoods());
            var kq = iCARS_ScoringService.CaculatorScore(menu, weatherContext);
            _masterMenuList = kq;
        }
        private async Task UpdateRecommendations(List<string> OrderedTags)
        {
            string dataWeatherRaw = await weatherService.FetchRawWeatherDataAsync();
            string weatherContext = weatherService.NormalizeWeatherContext(dataWeatherRaw);
            List<Food> menu = await Task.Run(() => db.GetFoods());
            var kq = iCARS_ScoringService.CaculatorScore(menu, weatherContext, hoadon, OrderedTags);
            _masterMenuList = kq;
            Application.Current.Dispatcher.Invoke(() =>
            {
                FilterMenu();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
