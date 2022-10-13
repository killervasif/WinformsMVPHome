﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsMVP.Models;
using WinFormsMVP.Views;

namespace WinFormsMVP.Presenters;

public class MainPresenter
{
    private readonly BindingSource _bindingSource;
    private readonly IMainView _mainView;
    private readonly IAddView _addView;
    private readonly IUpdateView _updateView;
    private readonly List<Student> _students;

    public MainPresenter(IMainView mainView, IAddView addView, IUpdateView updateView)
    {
        _mainView = mainView;
        _addView = addView;
        _updateView = updateView;

        _students = new List<Student>()
        {
            new ("Vasif","Babazade",new DateOnly(2004,10,2),7.8f),
            new ("Emin","Novruz",new DateOnly(2007,9,5),10.5f),
            new ("Resul","Qasimov",new DateOnly(2006,2,23),11.2f),
        };

        _bindingSource = new();

        _bindingSource.DataSource = _students;
        _mainView.SetStudentListBindingSource(_bindingSource);
        _mainView.SearchEvent += _mainView_SearchEvent;
        _mainView.DeleteEvent += _mainView_DeleteEvent;
        _mainView.AddEvent += _mainView_AddEvent;
        _mainView.UpdateEvent += _mainView_UpdateEvent;
    }


    private void _mainView_SearchEvent(object? sender, EventArgs e)
    {
        var searchValue = _mainView.SearchValue;

        if (!string.IsNullOrWhiteSpace(searchValue))
            _bindingSource.DataSource = _students.Where(s => s.FirstName.ToLower().Contains(searchValue.ToLower()) || s.LastName.ToLower().Contains(searchValue.ToLower())).ToList();
        else
            _bindingSource.DataSource = _students;
    }

    private void _mainView_DeleteEvent(object? sender, EventArgs e)
    {
        if (_bindingSource.Current is null)
            return;

        _bindingSource.Remove(_bindingSource.Current);
    }

    private void _mainView_AddEvent(object? sender, EventArgs e)
    {
        var result = ((Form)_addView).ShowDialog();

        if (result == DialogResult.Cancel)
            return;

        var student = new Student()
        {
            FirstName = _addView.FirstName,
            LastName = _addView.LastName,
            BirthDate = DateOnly.Parse(_addView.BirthDate.ToShortDateString()),
            Score = (float)_addView.Score
        };

        _bindingSource.Add(student);
    }

    private void _mainView_UpdateEvent(object? sender, EventArgs e)
    {
        if(_bindingSource.Current is null)
        {
            MessageBox.Show("Select Student To Update");
            return;
        }

        var result = ((Form)_updateView).ShowDialog();

        if (result == DialogResult.Cancel)
            return;

        var student = _bindingSource.Current as Student;

        student.FirstName = _updateView.FirstName;
        student.LastName = _updateView.LastName;
        student.BirthDate = DateOnly.Parse(_updateView.BirthDate.ToShortDateString());
        student.Score = (float)_updateView.Score;

        _bindingSource[_bindingSource.IndexOf(_bindingSource.Current)] = student;
    }

}
