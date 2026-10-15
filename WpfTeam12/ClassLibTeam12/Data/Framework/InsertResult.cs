namespace ClassLibTeam12.Data.Framework
{
    public class InsertResult : BaseResult
    {
        //NewId wordt teruggegeven door SQL Server - Auto Increment
        public int NewId { get; set; }
        public object Errors { get; set; }
    }
}