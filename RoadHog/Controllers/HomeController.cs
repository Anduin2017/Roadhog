using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DSHomework.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using DSHomework.Models;
using Microsoft.EntityFrameworkCore;

namespace DSHomework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<Sight> _finalRoute = new List<Sight>();

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Matrix()
        {
            var NameList = new List<string>(new string[1] { "" });
            var AllSights = _context.Sights.ToList();
            var AllRoutes = _context.Routes.ToList();
            AllSights.ForEach(t => NameList.Add(t.SightName));
            var List = new List<List<string>>(1) { NameList };
            foreach (var OneSight in AllSights)
            {
                var TempList = new List<string>();
                TempList.Add(OneSight.SightName);
                foreach (var TheSight in AllSights)
                {
                    TempList.Add(TheSight == OneSight
                        ? "0"
                        : AllRoutes
                        .SingleOrDefault(t => t.StartAt == OneSight && t.EndAt == TheSight)
                        ?.Length.ToString()
                        ?? "∞");
                }
                List.Add(TempList);
            }

            return View(List);
        }

        public IActionResult Travel()
        {
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        [HttpPost]
        public IActionResult Travel(Sight sight)
        {
            _finalRoute.Clear();
            _previsit(sight.SightId);
            string ResultOutput = string.Empty;
            _finalRoute.ForEach(t => ResultOutput += t.SightName + "->");
            ViewData["Result"] = ResultOutput.Trim('-', '>');
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        private void _previsit(string SightId)
        {
            if (_finalRoute.Contains(new Sight { SightId = SightId }))
            {
                return;
            }
            var StartPoint = _context
                .Sights
                .Include(t => t.StartRoutes)
                .SingleOrDefault(t => t.SightId == SightId);
            _finalRoute.Add(StartPoint);
            foreach (var Route in StartPoint.StartRoutes)
            {
                _previsit(Route.EndSightId);
            }
        }

        public IActionResult Graph()
        {
            return View();
        }

        public JsonResult GetResult()
        {
            var labels = new List<Label>();
            var stations = new List<Station>();
            var lines = new List<Line>();

            foreach (var Sight in _context.Sights)
            {
                var x = ran();
                var y = ran();

                labels.Add(new Label
                {
                    text = Sight.SightName,
                    x = x,
                    y = y - 20
                });

                stations.Add(new Station
                {
                    id = Convert.ToInt32(Sight.SightId),
                    name = Sight.SightName,
                    x = x,
                    y = y
                });
            }

            foreach (var line in _context.Routes)
            {
                lines.Add(new Line
                {
                    color = "#e52035",
                    name = line.Length.ToString(),
                    stations = new List<int>(2)
                    {
                        Convert.ToInt32(line.StartSightId),
                        Convert.ToInt32(line.EndSightId)
                    }
                });
            }

            return Json(new
            {
                labels = labels,
                stations = stations,
                lines = lines
            });
        }
        static int seed = DateTime.Now.GetHashCode();
        private int ran()
        {
            seed++;
            return new Random(DateTime.Now.GetHashCode() * seed.GetHashCode()).Next(0, 1000);
        }

        public IActionResult Path()
        {
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        public IActionResult Road()
        {
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        [HttpPost]
        public IActionResult Road(Sight sight)
        {
            _finalRoute.Clear();
            _previsit(sight.SightId);
            string ResultOutput = string.Empty;
            _finalRoute.ForEach(t => ResultOutput += t.SightName + "->");
            ViewData["Result"] = ResultOutput.Trim('-', '>');
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        [HttpPost]
        public IActionResult Path(Route model)
        {
            //从数据库取出所有点
            var AllSights = _context.Sights.Include(t => t.StartRoutes).ToList();
            var _startPlace = AllSights.SingleOrDefault(t => t.SightId == model.StartSightId);
            var _targetPlace = AllSights.SingleOrDefault(t => t.SightId == model.EndSightId);
            //对所有景点，都没去过，都无限远，都不知道前一步是谁
            foreach (var sight in AllSights)
            {
                sight.GoCost = int.MaxValue;
                sight.Gone = false;
                sight.PreviousSightId = null;
            }
            //自己去过了
            _startPlace.Gone = true;
            _startPlace.GoCost = 0;
            //自己周围那一圈，都去过了
            foreach (var _fromStart in _startPlace.StartRoutes)
            {
                var _nearPosition = AllSights.SingleOrDefault(t => t.SightId == _fromStart.EndSightId);
                _nearPosition.GoCost = _fromStart.Length;
                _nearPosition.PreviousSightId = _startPlace.SightId;
            }
            //当还没有去过目标点时
            while (_targetPlace.Gone == false)
            {
                //找出所有景区里没去过的最容易去的那个，
                //现在抵达它的距离一定是最小的，因为不可能有抵达它更近的方法了
                //所以可以直接接受这个点的
                var _newSight = AllSights.Where(t => t.Gone == false).OrderBy(t => t.GoCost).First();

                //遍历所有没去过的点，称其为未知点
                foreach (var _unknownSight in AllSights.Where(t => t.Gone == false))
                {
                    //如果新加的点里，存在一条路，它的目标是这个未知点
                    var _newRoute = _newSight.StartRoutes.SingleOrDefault(t => t.EndSightId == _unknownSight.SightId);
                    if (_newRoute != null)
                    {
                        //如果 新加的点 + 新加的点到这个未知点的距离 < 直接到这个未知点的距离
                        if (_newRoute.Length + _newSight.GoCost < _unknownSight.GoCost)
                        {
                            //更新这个未知点数据
                            _unknownSight.GoCost = _newRoute.Length + _newSight.GoCost;
                            _unknownSight.PreviousSightId = _newSight.SightId;
                        }
                    }
                }
                //好了，你算去过的了
                _newSight.Gone = true;
            }
            var _resultList = new List<string>();
            Sight _currentSight = AllSights.SingleOrDefault(t => t.SightId == _targetPlace.PreviousSightId);
            while (_currentSight.SightId != _startPlace.SightId)
            {
                _resultList.Insert(0, _currentSight.SightName);
                _currentSight = AllSights.SingleOrDefault(t => t.SightId == _currentSight.PreviousSightId);

            }
            string result = _startPlace.SightName + "->";
            _resultList.ForEach(t => result += t + "->");
            result += _targetPlace.SightName;
            ViewData["Result"] = result;
            ViewData["SightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }
    }
    public class Label
    {
        public string text;
        public double x;
        public double y;
    }
    public class Station
    {
        public int id;
        public string name;
        public double x;
        public double y;
    }
    public class Line
    {
        public string name;
        public string color;
        public List<int> stations;
    }


}
