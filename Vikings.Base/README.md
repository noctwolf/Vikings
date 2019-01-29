# Vikings.Base
- Vikings.Base is a base utility package, contains some mathematics, network and threading utility classes.

# Utility list
## Vikings.Threading.Tasks
- Delay utility class provide thread delayed starting 
### Usage: 
- Delay.IntervalAsync blocks current thread for a intervl and then calls your action   

		using Vikings.Threading.Tasks
		Delay.IntervalAsync(YourAction,MillisecondsBeforeCallYourAction)


