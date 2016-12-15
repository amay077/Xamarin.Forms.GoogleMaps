using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Animation.ValueAnimator;
using static Android.Animation.Animator;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class GenericAnimatorUpdateListner : Java.Lang.Object, IAnimatorListener, IAnimatorUpdateListener
    {
        public GenericAnimatorUpdateListner(Animator_ValueAnimatorHandler onUpdate, Animator_AnimatorHandler onStart, Animator_AnimatorHandler onEnd, Animator_AnimatorHandler onRepeat, Animator_AnimatorHandler onCancel, Action onDispose = null)
            :this(onUpdate, onDispose)
        {
            _OnStart = onStart;
            _OnEnd = onEnd;
            _OnRepeat = onRepeat;
            _OnCancel = onCancel;
        }
        public GenericAnimatorUpdateListner(Animator_ValueAnimatorHandler onUpdate, Action onDispose = null)
        {
            _OnUpdate = onUpdate;
            _OnDispose = onDispose;
        }
        private readonly Animator_ValueAnimatorHandler _OnUpdate;
        private readonly Action _OnDispose;
        private readonly Animator_AnimatorHandler _OnStart;
        private readonly Animator_AnimatorHandler _OnEnd;
        private readonly Animator_AnimatorHandler _OnRepeat;
        private readonly Animator_AnimatorHandler _OnCancel;

        public new void Dispose()
        {
            _OnDispose?.Invoke();
            base.Dispose();
        }


        public void OnAnimationUpdate(ValueAnimator animation)
            => _OnUpdate?.Invoke(animation);

        public void OnAnimationCancel(Animator animation)
            => _OnCancel?.Invoke(animation);

        public void OnAnimationEnd(Animator animation)
            => _OnEnd?.Invoke(animation);

        public void OnAnimationRepeat(Animator animation)
            => _OnRepeat?.Invoke(animation);

        public void OnAnimationStart(Animator animation)
            => _OnStart?.Invoke(animation);

        public delegate void Animator_ValueAnimatorHandler(ValueAnimator animation);
        public delegate void Animator_AnimatorHandler(Animator animation);
    }
}