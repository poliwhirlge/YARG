using Cysharp.Text;
using TMPro;
using UnityEngine;
using YARG.Helpers.Extensions;
using YARG.Menu;
using YARG.Scores;

namespace YARG.Menu.MoreInfo
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _score;
        [SerializeField]
        private TextMeshProUGUI _pctAndDiff;
        [SerializeField]
        private StarView _starView;

        public void SetScore(PlayerScoreRecord score)
        {
            using var b1 = ZString.CreateStringBuilder();
            b1.AppendFormat("{0:N0}", score.Score);
            _score.text = b1.ToString();

            var difficultyChar = score.Difficulty.ToChar();
            var percent = Mathf.Floor(score.Percent * 100f);
            using var b2 = ZString.CreateStringBuilder();
            b2.AppendFormat("{1:N0}% <b>{0}</b>", difficultyChar, percent);
            _pctAndDiff.text = b2.ToString();

            _starView.SetStars(score.Stars);
        }

        public void Reset()
        {
            _score.text = "-";
            _pctAndDiff.text = "";
            _starView.SetStars(0);
        }
    }


}
