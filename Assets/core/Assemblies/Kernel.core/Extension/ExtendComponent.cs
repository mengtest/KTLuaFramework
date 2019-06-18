//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Kernel.core
//{
//    public static class ExtendedComponent
//    {
//        public static T GetComponentEx<T>(this GameObject obj)
//            where T : Component
//        {
//            if (obj == null)
//            {
//                return null;
//            }

//            return obj.GetComponent<T>();
//        }

//        public static T[] GetComponentsInChildrenEx<T>(this GameObject obj, bool includeSelf = false,
//            bool includeInactive = true) where T : Component
//        {
//            if (obj == null)
//            {
//                return null;
//            }
//            IEnumerable<T> c = obj.GetComponentsInChildren<T>(includeInactive);
//            if (includeSelf)
//            {
//                IEnumerable<T> self = obj.GetComponents<T>();
//                c = c.Union(self).Where(com => includeInactive || com is MonoBehaviour && (com as MonoBehaviour).enabled);
//            }
//            return c.ToArray();
//        }

//        public static void SetActiveEx(this Component go, bool active)
//        {
//            if (go != null)
//            {
//                go.gameObject.SetActive(active);
//            }
//        }

//        public static void SetActiveEx(this UIWidget widget, bool active)
//        {
//            if (widget != null)
//            {
//                widget.gameObject.SetActiveEx(active);
//                widget.enabled = active;
//            }
//        }

//        public static void SetActiveEx(this GameObject go, bool active)
//        {
//            if (null != go)
//            {
//                go.SetActive(active);
//            }
//        }

//        public static void DestroyEx(this GameObject go, float time)
//        {
//            if (go != null)
//            {
//                GameObject.Destroy(go, time);
//            }
//        }
//        public static void DestroyEx(this Component go, float time)
//        {
//            if (go != null)
//            {
//                GameObject.Destroy(go.gameObject, time);
//            }
//        }
//        public static void DestroyEx(this UIWidget go, float time)
//        {
//            if (go != null)
//            {
//                GameObject.Destroy(go.gameObject, time);
//            }
//        }

//        public static void SetAlphaEx(this UIWidget widget, float a)
//        {
//            if (widget != null)
//            {
//                widget.alpha = a;
//            }
//        }

//        public static void SetColorEx(this UIWidget widget, Color color)
//        {
//            if (widget != null)
//            {
//                color.a = widget.color.a;
//                widget.color = color;
//            }
//        }

//        public static void SetColorEx(this UIWidget widget, uint color)
//        {
//            SetColorEx(widget, (Color)(Kernel.Engine.Color)color);
//        }

//        public static void SetColorIncludeAplhaEx(this UIWidget widget, Color color)
//        {
//            if (widget != null)
//            {
//                widget.color = color;
//            }
//        }

//        public static void SetDarkenEx(this UIWidget widget, bool darken)
//        {
//            if (darken)
//            {
//                widget.SetColorEx(Color.grey);
//            }
//            else
//            {
//                widget.SetColorEx(Color.white);
//            }
//        }

//        public static void SetDarkenEx(this GameObject obj, bool darken)
//        {
//            if (obj == null)
//            {
//                return;
//            }
//            foreach (var sprite in obj.GetComponentsInChildren<UIWidget>(true))
//            {
//                sprite.SetDarkenEx(darken);
//            }
//        }

//        public static void SetDarkenEx(this Component obj, bool darken)
//        {
//            if (obj == null)
//            {
//                return;
//            }
//            foreach (var sprite in obj.GetComponentsInChildren<UIWidget>(true))
//            {
//                sprite.SetDarkenEx(darken);
//            }
//        }

//        public static void SetGrayEx(this UIWidget widget, bool gray)
//        {
//            var sprite = widget as UIBasicSprite;
//            if (sprite != null)
//            {
//                sprite.grayScale = gray;
//            }


//            var label = widget as UILabel;
//            if (label != null)
//            {
//                label.grayScale = gray;
//            }
//        }

//        public static void SetGrayEx(this GameObject obj, bool gray)
//        {
//            if (obj == null) return;
//            foreach (var sprite in obj.GetComponentsInChildren<UIWidget>(includeInactive: true))
//            {
//                sprite.SetGrayEx(gray);
//            }
//        }

//        public static void SetGrayEx(this Component obj, bool gray)
//        {
//            if (obj == null) return;
//            foreach (var sprite in obj.GetComponentsInChildren<UIWidget>(includeInactive: true))
//            {
//                sprite.SetGrayEx(gray);
//            }
//        }

//        public static void SetNameEx(this GameObject go, string name)
//        {
//            if (null != go)
//            {
//                go.name = name;
//            }
//        }

//        public static void SetEnabledEx(this Collider2D comp, bool enable)
//        {
//            if (comp != null)
//            {
//                comp.enabled = enable;
//                var uiButton = comp.GetComponent<UIButton>();
//                if (uiButton != null)
//                {
//                    uiButton.SetState(enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
//                }
//            }
//        }

//        public static void SetColliderEnabledEx(this Component comp, bool enable)
//        {
//            if (comp != null)
//            {
//                var collider = comp.GetComponent<Collider2D>();
//                SetEnabledEx(collider, enable);
//            }
//        }
//        public static void SetGray(this Component comp, bool enable)
//        {
//            foreach (var sprite in comp.GetComponentsInChildren<UIWidget>(includeInactive: true))
//            {
//                sprite.SetGrayEx(enable);
//            }
//        }
//        public static void DestoryImmediateColliderEx(this Component comp)
//        {
//            if (comp != null)
//            {
//                var collider = comp.GetComponent<Collider2D>();
//                if (collider != null)
//                {
//                    GameObject.DestroyImmediate(collider);
//                }
//            }
//        }
//        public static void SetTweenerEnabledEx(this Component comp, bool enable)
//        {
//            if (comp != null)
//            {
//                var tweener = comp.GetComponents<UITweener>();
//                for (int i = 0; i < tweener.Length; i++)
//                {
//                    if (tweener[i] != null) tweener[i].enabled = enable;
//                }
//            }
//        }

//        public static bool GetIsPrefab(this Component comp)
//        {
//            return comp?.gameObject.scene.rootCount == 0;
//        }
//	}
//}