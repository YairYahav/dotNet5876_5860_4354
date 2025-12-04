namespace BlApi;
using BO;


public interface IAdmin
{
    void ResetDB();
    void InitializeDB();
    DateTime GetClock();
    void ForwardClock(TimeUnit unit);
    BO.Config GetConfig();
    void SetConfig(BO.Config config);


    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
}