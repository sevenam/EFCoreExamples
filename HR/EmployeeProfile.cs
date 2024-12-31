﻿namespace HR;
public class EmployeeProfile
{
    public int Id { get; set;}
    public string Phone  { get;set;   }
    public string Email {   get;set;  }

    public int EmployeeId { get; set;}
    public Employee Employee  { get; set;} = null!;
}
