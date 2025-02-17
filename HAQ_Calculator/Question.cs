﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HAQ_Calculator
{
    public class Question
    {
        private readonly string _questionText;
        private readonly List<string> _answers;
        private readonly Panel _stack;
        private readonly Chapters _chapter;
        private readonly HaqCalculator _haqCalculator;
        private int _point;
        private readonly int _questionNum;

        public Question(string questionText, HaqCalculator haqCalculator, Chapters chapter, int questionNum,
            List<string> answers = null)
        {
            _questionText = questionText;
            _haqCalculator = haqCalculator;
            _answers = answers;
            _questionNum = questionNum;
            _chapter = chapter;
        }

        public Decorator GetControl()
        {
            var mainBorder = new Border
            {
                BorderBrush = Brushes.Black,
                CornerRadius = new CornerRadius(7),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                UseLayoutRounding = true
            };
            var mainStack = new StackPanel();

            var stackAnswers = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            if (_answers == null)
            {
                for (var i = 0; i < 4; i++)
                {
                    var button = new RadioButton
                    {
                        Content = i,
                        FontSize = 16,
                        VerticalAlignment = VerticalAlignment.Center,
                        IsChecked = false
                    };
                    button.Checked += ButtonOnChecked;
                    stackAnswers.Children.Add(button);
                }
            }
            else
            {
                var i = 0; 
                stackAnswers.Orientation = Orientation.Vertical;
                foreach (var answer in _answers)
                {
                    var button = new CheckBox()
                    {
                        Content = answer,
                        Name = $"A{i}",
                        FontSize = 16,
                        IsChecked = false
                    };
                    button.Click += ButtonOnClick;
                    stackAnswers.Children.Add(button);
                    i++;
                }
            }

            mainStack.Children.Add(new TextBlock
                {Text = _questionText, FontSize = 16, Margin = new Thickness(0, 0, 10, 0)});
            mainStack.Children.Add(stackAnswers);
            mainBorder.Child = mainStack;

            return mainBorder;
        }
        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            var answerNum = int.Parse((sender as CheckBox)!.Name.Remove(0, 1));
            switch (_chapter)
            {
                case Chapters.FirstHalfFixtures:
                    if (answerNum == _answers.Count - 1 && (sender as CheckBox)?.IsChecked == true)
                        ((sender as CheckBox)!.Parent as StackPanel)!.Children.Add(new TextBox());
                    else if (answerNum == _answers.Count - 1 && (sender as CheckBox)?.IsChecked == false)
                        ((sender as CheckBox)!.Parent as StackPanel)!.Children.RemoveAt(
                            ((sender as CheckBox)!.Parent as StackPanel)!.Children.Count - 1);
                    SetForHalfPoints(_haqCalculator.FirstHalf,
                        (sender as CheckBox)?.IsChecked == true);
                    break;
                case Chapters.FirstHalfNeedHelp:
                    switch (answerNum)
                    {
                        case 0:
                            SetQuestionPointsValue(_haqCalculator.DressingAndPersonalCare,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 1:
                            SetQuestionPointsValue(_haqCalculator.GettingUp,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 2:
                            SetQuestionPointsValue(_haqCalculator.Meal,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 3:
                            SetQuestionPointsValue(_haqCalculator.Walks,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                    }
                    break;
                case Chapters.SecondHalfFixtures:
                    if (answerNum == _answers.Count - 1 && (sender as CheckBox)?.IsChecked == true)
                        ((sender as CheckBox)!.Parent as StackPanel)!.Children.Add(new TextBox());
                    else if (answerNum == _answers.Count - 1 && (sender as CheckBox)?.IsChecked == false)
                        ((sender as CheckBox)!.Parent as StackPanel)!.Children.RemoveAt(
                            ((sender as CheckBox)!.Parent as StackPanel)!.Children.Count - 1);
                    SetForHalfPoints(_haqCalculator.SecondHalf,
                        (sender as CheckBox)?.IsChecked == true);
                    break;
                case Chapters.SecondHalfNeedHelp:
                    switch (answerNum)
                    {
                        case 0:
                            SetQuestionPointsValue(_haqCalculator.Hygiene,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 1:
                            SetQuestionPointsValue(_haqCalculator.AchievableRange,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 2:
                            SetQuestionPointsValue(_haqCalculator.PowerBrushes,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                        case 3:
                            SetQuestionPointsValue(_haqCalculator.OtherActivities,
                                (sender as CheckBox)?.IsChecked == true);
                            break;
                    }
                    break;
            }
        }

        private void SetForHalfPoints(Chapter chapter, bool isIncrease)
        {
            if (isIncrease)
            {
                chapter.TotalPoints++;
                _haqCalculator.TotalPoints++;
            }
            else
            {
                chapter.TotalPoints--;
                _haqCalculator.TotalPoints--;
            }

            _haqCalculator.Haq = _haqCalculator.TotalPoints / _haqCalculator.IncludeChapters;
        }
        
        private void SetQuestionPointsValue(Chapter chapter, bool isIncrease)
        {
            if (isIncrease)
            {
                chapter.AdditionalPoints++;
                for (var i = 0; i < chapter.QuestionsPoints.Count; i++)
                    chapter.QuestionsPoints[i]++;
            }
            else
            {
                chapter.AdditionalPoints--;
                for (var i = 0; i < chapter.QuestionsPoints.Count; i++)
                    chapter.QuestionsPoints[i]--;
            }
            
            _haqCalculator.TotalPoints -= chapter.TotalPoints;
            chapter.TotalPoints = chapter.QuestionsPoints.Max();
            _haqCalculator.TotalPoints += chapter.TotalPoints;
            _haqCalculator.Haq = _haqCalculator.TotalPoints / _haqCalculator.IncludeChapters;
        }

        private void SetQuestionPointsValue(Chapter chapter, int value)
        {
            _haqCalculator.TotalPoints -= chapter.TotalPoints;
            chapter.QuestionsPoints[_questionNum] = value + chapter.AdditionalPoints;
            chapter.TotalPoints = chapter.QuestionsPoints.Max();
            _haqCalculator.TotalPoints += chapter.TotalPoints;
            _haqCalculator.Haq = _haqCalculator.TotalPoints / _haqCalculator.IncludeChapters;
        }

        private void ButtonOnChecked(object sender, RoutedEventArgs e)
        {
            var intResult = int.Parse((sender as RadioButton)!.Content.ToString()!);
            switch (_chapter)
            {
                case Chapters.DressingAndPersonalCare:
                    SetQuestionPointsValue(_haqCalculator.DressingAndPersonalCare, intResult);
                    break;
                case Chapters.GettingUp:
                    SetQuestionPointsValue(_haqCalculator.GettingUp, intResult);
                    break;
                case Chapters.Meal:
                    SetQuestionPointsValue(_haqCalculator.Meal, intResult);
                    break;
                case Chapters.Walks:
                    SetQuestionPointsValue(_haqCalculator.Walks, intResult);
                    break;
                case Chapters.Hygiene:
                    SetQuestionPointsValue(_haqCalculator.Hygiene, intResult);
                    break;
                case Chapters.AchievableRange:
                    SetQuestionPointsValue(_haqCalculator.AchievableRange, intResult);
                    break;
                case Chapters.PowerBrushes:
                    SetQuestionPointsValue(_haqCalculator.PowerBrushes, intResult);
                    break;
                case Chapters.OtherActivities:
                    SetQuestionPointsValue(_haqCalculator.OtherActivities, intResult);
                    break;
            }
        }
    }
}