using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    public bool Done { get;}
    public bool Execute();
    public bool Undo();
}
