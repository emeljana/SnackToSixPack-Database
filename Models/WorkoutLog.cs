namespace SnackToSixPack.Classes;
using System.ComponentModel.DataAnnotations;

public class WorkoutLog
{
    [Key]
    public int UserId  { get; set; }
}