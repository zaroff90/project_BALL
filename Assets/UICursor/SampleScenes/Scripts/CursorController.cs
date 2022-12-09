using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooldhands;

namespace Cooldhands.UICursor.Example
{
    public class CursorController : UICursorBehaviour
    {
        public Animator m_animator;
        private void OnEnable()
        {
            GameObject.Find("EventSystem").GetComponent<UICursorCustomInputSystem>().m_cursor = this;
        }
        public override void OnClickableElementChanged(){
            if(m_animator != null && m_animator.isActiveAndEnabled)
            {
                m_animator.SetBool("Hover", base.IsOverClickeableElement);
            }
        }
        public override void OnClick()
        {
            if(m_animator != null && m_animator.isActiveAndEnabled)
            {
                m_animator.SetTrigger("Click");
            }
        }
        private void Update()
        {

        }
    }
}
