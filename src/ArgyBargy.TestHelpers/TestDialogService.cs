using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using XText;

namespace ArgyBargy.TestHelpers
{
    public class TestDialogService : IDialogService
    {
        private readonly Queue<object> results = new Queue<object>();

        public ReadOnlyCollection<object> DialoguesShown
        {
            get { return dialoguesShown.AsReadOnly(); }
        }

        private readonly List<object> dialoguesShown = new List<object>();

        public Window GetCurrentTopmostWindow()
        {
            return null;
        }

        public DialogueResult<TResult> ShowDialogue<TResult>(IDialogueView<TResult> dialogueView)
        {
            var result = ResolveFormResult<TResult>(dialogueView);
            dialogueView.DialogueDisplayed(new TestDialogueWindow());
            dialoguesShown.Add(dialogueView);
            return result;
        }

        public DialogueResult ShowDialogue(IDialogueViewWithoutResult dialogueView)
        {
            var formTypeName = GetFormTypeName(dialogueView);
            dialoguesShown.Add(dialogueView);
            if (results.Count > 0)
            {
                var result = results.Dequeue();

                if (result.GetType() == typeof(DialogueResult))
                    return (DialogueResult)result;

                return new DialogueResult();
            }

            throw new InvalidOperationException(string.Format("No DialogueResult on stack, view of type {0} expects result of type DialogueResult", formTypeName));
        }

        public IBusyView ShowBusy()
        {
            return new FakeBusyView();
        }

        public void HideBusy()
        {
        }

        public DialogueResult ShowActionsDialogue(string title, XSection message = null, FrameworkElement content = null,
                                                  HorizontalAlignment titleAlignment = HorizontalAlignment.Left,
                                                  HorizontalAlignment buttonsAlignment = HorizontalAlignment.Right, params DetailsAction[] buttons)
        {
            dialoguesShown.Add(title);

            if (results.Count > 0)
            {
                var result = results.Dequeue();

                if (result.GetType() == typeof(ActionsDialogueResult))
                {

                    var actionsDialogueResult = (ActionsDialogueResult)result;

                    if (actionsDialogueResult.Cancelled)
                        return actionsDialogueResult;

                    try
                    {
                        buttons.Single(b => b.AutomationId == actionsDialogueResult.ButtonAutomationId).Command.Execute(null);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidOperationException(
                            string.Format("No button with AutomationId of {0} on {1}", actionsDialogueResult.ButtonAutomationId, title));
                    }
                    return actionsDialogueResult;
                }

                throw new InvalidOperationException(string.Format("Expecting ActionsDialogueResult for dialogue with title {0}, found {1}", title, result.GetType().Name));
            }

            throw new InvalidOperationException(string.Format("No ActionsDialogueResult on stack for action dialogue with title {0}", title));
        }

        public DialogueResult ShowCommandLinkDialogueThenExecuteAction(string title, XSection message, params DetailsAction[] detailsActions)
        {
            dialoguesShown.Add(title);

            if (results.Count > 0)
            {
                var result = results.Dequeue();

                if (result.GetType() == typeof(ActionsDialogueResult))
                {
                    var actionsDialogueResult = (ActionsDialogueResult)result;
                    if (actionsDialogueResult.Cancelled)
                        return actionsDialogueResult;

                    var detailsAction = detailsActions.SingleOrDefault(b => b.AutomationId == actionsDialogueResult.ButtonAutomationId);
                    if (detailsAction == null)
                        throw new InvalidOperationException(string.Format("No button with AutomationId of {0} on {1}", actionsDialogueResult.ButtonAutomationId, title));


                    detailsAction.Command.Execute(null);

                    return actionsDialogueResult;
                }

                throw new InvalidOperationException(string.Format("Expecting ActionsDialogueResult for dialogue with title {0}, found {1}", title, result.GetType().Name));
            }

            throw new InvalidOperationException(string.Format("No ActionsDialogueResult on stack for action dialogue with title {0}", title));
        }

        private static string GetFormTypeName(object form)
        {
            var typeName = form.GetType().Name;
            var indexOf = typeName.IndexOf("Proxy");
            return indexOf == -1 ? typeName : typeName.Substring(0, indexOf);
        }

        private DialogueResult<T> ResolveFormResult<T>(object form)
        {
            var formTypeName = GetFormTypeName(form);

            if (results.Count > 0)
            {
                var result = results.Dequeue();

                //Check if form is a DialogueView
                if (result is DialogueResult<T>)
                    return (DialogueResult<T>)result;

                if (result is T)
                    return new DialogueResult<T>((T)result);

                throw new InvalidOperationException(
                    string.Format(
                        "Found result {0}, expecting DialogueResult<{1}> or {1}. AddResult calls may be in wrong order.",
                        GetFormTypeName(result),
                        typeof(T).Name));
            }

            if (typeof(T) == typeof(MessageBoxResult))
                throw new InvalidOperationException("No DialogueResult on stack, expected MessageBoxResult");

            throw new InvalidOperationException(string.Format("No DialogueResult on stack, view of type {0} expects result of type DialogueResult<{1}> or {1}", formTypeName, typeof(T).Name));
        }


        /// <summary>
        /// The instance of the dialog, or the message box title.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dialogue"></param>
        public void AssertWasShown<T>(T dialogue)
        {
            if (!DialoguesShown.Contains(dialogue))
                throw new AssertFailedException(string.Format("Expected {0} dialogue to be shown", typeof(T).Name));
        }

        /// <summary>
        /// The instance of the dialog, or the message box title.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dialogue"></param>
        public void AssertWasNotShown<T>(T dialogue)
        {
            if (DialoguesShown.Contains(dialogue))
                throw new AssertFailedException(string.Format("Did not expected {0} dialogue to be shown", typeof(T).Name));
        }

        /// <summary>
        /// Asserts a form was shown
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AssertFormWasShown<T>()
        {
            if (DialoguesShown.Any(d => d.GetType() != typeof(T)))
                throw new AssertFailedException(string.Format("Expected form of type {0} to be shown", typeof(T).Name));
        }

        /// <summary>
        /// Or DialogueResult&lt;T&gt; / DialogueResult (if view has no result)
        /// Or Result which will be wrapped in a DialogueResult
        /// </summary>
        /// <param name="result"></param>
        public void AddResult(object result)
        {
            //When a view of the wrong type is added to the stack, fail due to incorrect ordering
            var interfaces = result.GetType().GetInterfaces();
            if (interfaces.Any(IsDialogueView))
            {
                var dialogueWithoutResult = interfaces.SingleOrDefault(IsDialogueViewWithoutResult);
                if (dialogueWithoutResult != null)
                    throw new InvalidOperationException(string.Format("{0} is a IDialogueViewWithoutResult, register the DialogueResult, or null for cancelled result", result.GetType()));

                var dialogueViewType = interfaces.Single(IsDialogueViewOfType);
                var valueTypeName = dialogueViewType.GetGenericArguments()[0].Name;
                throw new InvalidOperationException(string.Format("{0} is a {1}, register the DialogueResult<{2}>, or a {2} result instead",
                    result.GetType().Name,
                    dialogueViewType.Name.Substring(0, dialogueViewType.Name.Length - 2) + "<" + valueTypeName + ">",
                    valueTypeName));
            }

            results.Enqueue(result);
        }

        public void AssertAllExpectedDialoguesShown()
        {
            if (results.Count != 0)
                throw new AssertFailedException(string.Format("Expected {0} more dialog to be shown", results.Count));
        }

        private static bool IsDialogueView(Type i)
        {
            return IsDialogueViewOfType(i) || IsDialogueViewWithoutResult(i);
        }

        private static bool IsDialogueViewOfType(Type i)
        {
            return (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDialogueView<>));
        }

        private static bool IsDialogueViewWithoutResult(Type i)
        {
            return i == typeof(IDialogueViewWithoutResult);
        }
    }
}
