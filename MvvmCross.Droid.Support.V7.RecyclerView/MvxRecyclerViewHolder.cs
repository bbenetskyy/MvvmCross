// MvxRecyclerViewHolder.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Windows.Input;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;

namespace MvvmCross.Droid.Support.V7.RecyclerView
{
    public class MvxRecyclerViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder, IMvxRecyclerViewHolder, IMvxBindingContextOwner
    {
        private readonly IMvxBindingContext _bindingContext;

        private object _cachedDataContext;
        private ICommand _click, _longClick;
        private bool _clickOverloaded, _longClickOverloaded;

        public IMvxBindingContext BindingContext
        {
            get { return _bindingContext; }
            set { throw new NotImplementedException("BindingContext is readonly in the list item"); }
        }

        public object DataContext
        {
            get { return _bindingContext.DataContext; }
            set { _bindingContext.DataContext = value; }
        }

        public ICommand Click
        {
            get { return this._click; }
            set { this._click = value; if (this._click != null) this.EnsureClickOverloaded(); }
        }

        private void EnsureClickOverloaded()
        {
            if (this._clickOverloaded)
                return;
            this._clickOverloaded = true;
            this.ItemView.Click += (sender, args) => ExecuteCommandOnItem(this.Click);
        }

        public ICommand LongClick
        {
            get { return this._longClick; }
            set { this._longClick = value; if (this._longClick != null) this.EnsureLongClickOverloaded(); }
        }

        private void EnsureLongClickOverloaded()
        {
            if (this._longClickOverloaded)
                return;
            this._longClickOverloaded = true;
            this.ItemView.LongClick += (sender, args) => ExecuteCommandOnItem(this.LongClick);
        }

        protected virtual void ExecuteCommandOnItem(ICommand command)
        {
            if (command == null)
                return;

            var item = DataContext;
            if (item == null)
                return;

            if (!command.CanExecute(item))
                return;

            command.Execute(item);
        }

        public MvxRecyclerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView)
        {
            this._bindingContext = context;
        }

        public MvxRecyclerViewHolder(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {}

        public void OnAttachedToWindow()
        {
            if (_cachedDataContext != null && DataContext == null)
                DataContext = _cachedDataContext;
        }

        public void OnDetachedFromWindow()
        {
            _cachedDataContext = DataContext;
            DataContext = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bindingContext.ClearAllBindings();
                _cachedDataContext = null;
            }

            base.Dispose(disposing);
        }
    }
}