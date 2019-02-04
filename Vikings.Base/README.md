# Vikings.Base
- Vikings.Base is a base utility package, contains some mathematics, network and threading utility classes.

# Utility list
## Vikings.Threading.Tasks
- Delay utility class provide action delayed starting 
### Usage: 
- Delay.IntervalAsync Do not block the current thread for a interval and then calls your action
- 不阻塞当前线程，按照固定的时间间隔调用你的操作，在时间到达之前的多次操作仅保留最后一次。
比如，每秒刷新一次界面上的进度条，在一秒内多次调用这个方法，但是实际只执行最后一次，之前的操作都被忽略。
```C#
Vikings.Threading.Tasks.Delay.IntervalAsync(() => progressBar.Value = downloadLength);
```
