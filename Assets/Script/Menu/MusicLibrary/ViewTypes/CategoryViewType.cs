using System;
using System.Collections.Generic;
using System.Linq;
using YARG.Core.Song;
using YARG.Song;

namespace YARG.Menu.MusicLibrary
{
    public class CategoryViewType : ViewType
    {
        public override BackgroundType Background => BackgroundType.Category;

        public readonly string SourceCountText;
        public readonly string CharterCountText;
        public readonly string GenreCountText;
        public readonly string SubgenreCountText;

        protected readonly string Primary;

        protected readonly int SongCount;
        private readonly Action _clickAction;

        private static readonly HashSet<string> SourceCounter  = new();
        private static readonly HashSet<string> CharterCounter = new();
        private static readonly HashSet<string> GenreCounter   = new();
        private static readonly HashSet<string> SubgenreCounter = new();
        public CategoryViewType(string primary, int songCount, SongEntry[] songsUnderCategory,
            Action clickAction = null)
        {
            Primary = primary;
            SongCount = songCount;
            _clickAction = clickAction;

            foreach (var song in songsUnderCategory)
            {
                SourceCounter.Add(song.Source);
                CharterCounter.Add(song.Charter);
                GenreCounter.Add(song.Genre);
                if (!string.IsNullOrEmpty(song.Subgenre))
                {
                    SubgenreCounter.Add(song.Subgenre);
                }
            }

            SourceCountText = $"{SourceCounter.Count} Source{(SourceCounter.Count == 1 ? "" : "s")}";
            CharterCountText = $"{CharterCounter.Count} Charter{(CharterCounter.Count == 1 ? "" : "s")}";
            GenreCountText = $"{GenreCounter.Count} Genre{(GenreCounter.Count == 1 ? "" : "s")}";
            SubgenreCountText = $"{SubgenreCounter.Count} Subgenre{(SubgenreCounter.Count == 1 ? "" : "s")}";
            SourceCounter.Clear();
            CharterCounter.Clear();
            GenreCounter.Clear();
            SubgenreCounter.Clear();
        }

        public override string GetPrimaryText(bool selected)
        {
            return FormatAs(Primary, TextType.Bright, selected);
        }

        public override string GetSideText(bool selected)
        {
            return CreateSongCountString(SongCount);
        }

        public override void PrimaryButtonClick()
        {
            _clickAction?.Invoke();
        }
    }
}