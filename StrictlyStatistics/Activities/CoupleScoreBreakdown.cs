﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using StrictlyStatistics.Adapters;

namespace StrictlyStatistics.Activities
{
    [Activity(Label = "CoupleScoreBreakdown")]
    public class CoupleScoreBreakdown : StrictlyStatsActivity
    {
        public ListView CouplesList { get; set; }
        public Spinner CoupleInput { get; set; }
        public int SelectedCoupleId { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CoupleScoreBreakdown);

            InitialiseCoupleInput();
        }

        void PopulateListView()
        {
            CouplesList = FindViewById<ListView>(Resource.Id.dancesListView);

            var scores = Repo.GetAllScores().Where(x => x.CoupleID == SelectedCoupleId);
            var dances = Repo.GetAllDances();

            var couplesDances = new List<Tuple<string, int>>();
            couplesDances.Add(new Tuple<string, int>("Average dance score", CouplesAverageScore()));

            foreach (var s in scores)
            {
                var d = dances.FirstOrDefault(x => x.DanceId == s.DanceID);
                couplesDances.Add(new Tuple<string, int>(d.Name, s.ScoreValue));
            }

            var adapter = new SimpleListItem2ListAdapter(this, couplesDances);
            CouplesList.Adapter = adapter;
        }

        void InitialiseCoupleInput()
        {
                CoupleInput = FindViewById<Spinner>(Resource.Id.couplesScorebreakdownInput);
                var couples = Repo.GetAllCouples().ToList();
                var couplesNames = couples.Select(x => x.CoupleName).ToList();
                couplesNames.Insert(0, "Select couple");
                CoupleInput.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, couplesNames);

                CoupleInput.ItemSelected += (sender, e) =>
                {
                    Spinner spinner = (Spinner)sender;
                    var selected = spinner.GetItemAtPosition(e.Position);
                    SelectedCoupleId = couples.FirstOrDefault(x => selected.ToString().Contains(x.CoupleName))?.CoupleID ?? 0;

                    if(SelectedCoupleId != 0)
                        PopulateListView();
                };                            
        }

        int CouplesAverageScore()
        {
            var couplesScores = Repo.GetAllScores().Where(x => x.CoupleID == SelectedCoupleId);
            return (int)couplesScores.Average(x => x.ScoreValue);
        }
    }
}