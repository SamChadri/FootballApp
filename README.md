# FootballApp
University of Illinois at Urbana-Champaign  Football Data Application

  (I don't really know who actually came up with the idea for this thing, so I will just give the credit to Tao Xie and Lovie Smith, the two names I remember.)

## Functionality

  This app serves as a data access and storage tool for the coaches at U of I. The data attributes and specific details are designed to wishes of the coaches(lol I hope).

  However, since a large sample of play data was not provided, the stats displayed had to be randomly generated. The SampleData.cs file implements a majority of the functions needed to generate the random data for all the stats shown. That file utilizes the Stats.cs file which will hopefully generate  real stats once enough data has been inputed.
  
### Data Input

  There are two methods of data entry implemented. The **Create** route from the home screeen presents an option to create either a game or a play. Default Games and Players should be inserted into the database upon the app launch. 
  
  An import option from a Excel file will be implemented at a later date.
  
 ### User Authentication
 
  In order to use the app, you must login using Microsoft Passport Authentication. However, you must first setup a PIN in your Windows Settings. Once that is done, you can register as a player or coach.
  
### User Authorization 

  The backend can support this functionality, but it has not been implemented (Mostly cuz I don't remeber what they wanted).
  
### Data Retreival 

  The **Season** route from the home screen displays stats from the whole season, but can be narrowed down as the app is explored. The stats displayed are mostly randomly generated.
  
  The **Profile** route displays the current user stats. Although the QB is used as a default, further functionality will allow coaches to view the whole roster and stats for each player.
  
 ## Further Implementation
 
 Features to be implemented at a later date.
 
- Full Authorization
- File Importation
- Roster Retreival/Display
- Probably alot of other stuff they wanted, but I lowkey forgot
 
 
 Well...I think thats it folks. Excuse my spelling, Grammarly is buggin on this site.
  
  
