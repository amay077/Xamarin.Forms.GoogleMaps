using System;

namespace Xamarin.Forms.GoogleMaps.iOS
{
/*
    public class GenericAnimatorUpdateListner : IAnimatorListener, IAnimatorUpdateListener
    {
        public GenericAnimatorUpdateListner(Animator_ValueAnimatorHandler onUpdate, Animator_AnimatorHandler onStart, Animator_AnimatorHandler onEnd, Animator_AnimatorHandler onRepeat, Animator_AnimatorHandler onCancel, Action onDispose = null)
            : this(onUpdate, onDispose)
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

        public IntPtr Handle { get; set; }

        public void Dispose()
            => _OnDispose?.Invoke();


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
    */
}