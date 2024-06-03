using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YARG.Menu;
using YARG.Menu.Navigation;

namespace YARG
{
    public class MoreInfoMenuButton : MonoBehaviour
    {
        [SerializeField]
        public Image Icon;
        [SerializeField]
        public TextMeshProUGUI Text;
        [SerializeField]
        public NavigatableButton Button;
    }
}
