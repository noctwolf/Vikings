# Vikings.Base
- Vikings.Base is a base utility package, contains some mathematics, network and threading utility classes.

# Utility list
## Vikings.Threading.Tasks
- Delay utility class provide action delayed starting 
### Usage: 
- Delay.IntervalAsync Do not block the current thread for a interval and then calls your action
- 不阻塞当前线程，按照固定的时间间隔调用你的操作。比如，每秒刷新一次界面上的进度条
```C#
Vikings.Threading.Tasks.Delay.IntervalAsync(() => progressBar.Value = downloadLength);
```
