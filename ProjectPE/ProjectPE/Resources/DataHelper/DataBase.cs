using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Util;
using ProjectPE.Resources.Model;
using SQLiteNetExtensions.Extensions;

namespace ProjectPE.Resources.DataHelper
{

    class DataBase
    {
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public bool createDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    connection.CreateTable<dbPlaylist>();
                    connection.CreateTable<dbExercise>();
                    connection.CreateTable<dbPlaylistExercise>();

                    loadData(connection);
                    //resetTables(connection);
                    return true;                    
                }
            }
            catch(SQLiteException ex)
            {
                Log.Info("SQliteEX: ", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTablePlaylist(dbPlaylist playlist)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    connection.Insert(playlist);
                    return true;
                }                
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTableExercise(dbExercise exercise)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    connection.Insert(exercise);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public List<dbPlaylist> selectTablePlaylists()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {                    
                   //return connection.Table<dbPlaylist>().ToList();
                   return connection.GetAllWithChildren<dbPlaylist>(null, true).ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public List<dbExercise> selectTableExercise()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    return connection.GetAllWithChildren<dbExercise>(null, true).ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public List<dbExercise> getPlaylistExercises(string playlistName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //return connection.GetAllWithChildren<dbPlaylist>(x => x.name == playlistName).ToList();
                    //return connection.getwh\
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where name=?", playlistName);
                    var tempPlaylist = connection.GetWithChildren<dbPlaylist>(playlist[0].id);
                    var tempExerciseList = tempPlaylist.exercises;
                    return tempExerciseList;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public List<dbExercise> getTargetAreaExercises(string targetName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //return connection.GetAllWithChildren<dbPlaylist>(x => x.name == playlistName).ToList();
                    //return connection.getwh\
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise where targetArea=?", targetName);
                    //var tempPlaylist = connection.GetWithChildren<dbPlaylist>(playlist[0].id);
                    //var tempExerciseList = tempPlaylist.exercises;
                    return exerciseList;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public dbExercise getExercise(string exerciseName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //return connection.GetAllWithChildren<dbPlaylist>(x => x.name == playlistName).ToList();
                    //return connection.getwh\
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise where name=?", exerciseName);
                    //var tempPlaylist = connection.GetWithChildren<dbPlaylist>(playlist[0].id);
                    //var tempExerciseList = tempPlaylist.exercises;
                    return exerciseList[0];
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public bool deletePlaylist(string playlistName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where name=?", playlistName);
                    connection.Delete(playlist[0]);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public bool deleteExercise(string exerciseName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    var exercise = connection.Query<dbExercise>("SELECT * FROM dbExercise where name=?", exerciseName);
                    connection.Delete(exercise[0]);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public List<string> getPlaylistFavorites()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //get playlist favorites
                    var playlistNames = new List<string>();
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where favorite=?", true);

                    foreach (dbPlaylist name in playlist)
                    {
                        playlistNames.Add(name.name);
                    }

                    return playlistNames;
                    
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        //Get Upper body and Lower body playlists are just to show the the potential of 
        //multiple categories on the home page.
        public List<string> getUpperBodyPlaylists()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //get playlist favorites
                    var playlistNames = new List<string>();
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where playlistType=?", "Upper Body");

                    foreach (dbPlaylist name in playlist)
                    {
                        playlistNames.Add(name.name);
                    }

                    return playlistNames;

                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public List<string> getLowerBodyPlaylists()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //get playlist favorites
                    var playlistNames = new List<string>();
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where playlistType=?", "Lower Body");

                    foreach (dbPlaylist name in playlist)
                    {
                        playlistNames.Add(name.name);
                    }

                    return playlistNames;

                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public bool updateFavorite(string name, bool isFavorite)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    connection.Query<dbPlaylist>("UPDATE dbPlaylist SET favorite=? WHERE name=?", isFavorite, name);
                    return true;

                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public bool updatePlaylistName(string name, string newName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where name=?", name);

                    playlist[0].name = newName;
                    if (connection.Update(playlist[0]) > 0)
                        return true;
                    else
                        return false;
                    //connection.Query<dbPlaylist>("UPDATE dbPlaylist SET name=? WHERE name=?", newName, name);               
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        } 
        
        public bool updateExercise(string name, string newName, string set, string rep, string weight, string tarArea, string equip)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise where name=?", name);
                    var exercise = exerciseList[0];

                    exercise.name = newName ;
                    exercise.sets = set;
                    exercise.reps = rep;
                    exercise.weight = weight;
                    exercise.targetArea = tarArea;
                    exercise.equipment = equip;

                    if (connection.Update(exercise) > 0)
                        return true;
                    else
                        return false;
                                 
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return false;
            }
        }

        public void removeExerciseFromPlaylist (string playlistName, string exerciseName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //find the user specified playlist and exercise
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where name=?", playlistName);
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise where name=?", exerciseName);

                    //
                    var playlistWithExercises = connection.GetWithChildren<dbPlaylist>(playlist[0].id);
                    var exercise = exerciseList[0];
                    //remove the exercise from the playlist
                    playlistWithExercises.exercises.RemoveAll(item => item.id == exercise.id);

                    //return true if update was successful
                    connection.UpdateWithChildren(playlistWithExercises);
                                        
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                
            }
        }

        public void addExerciseFromPlaylist(string playlistName, string exerciseName)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //find the user specified playlist and exercise
                    var playlist = connection.Query<dbPlaylist>("SELECT * FROM dbPlaylist where name=?", playlistName);
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise where name=?", exerciseName);

                    //
                    var playlistWithExercises = connection.GetWithChildren<dbPlaylist>(playlist[0].id);
                    var exercise = exerciseList[0];
                    //remove the exercise from the playlist
                    //playlistWithExercises.exercises.RemoveAll(item => item.id == exercise.id);
                    playlistWithExercises.exercises.Add(exercise);
                    //return true if update was successful
                    connection.UpdateWithChildren(playlistWithExercises);

                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);

            }
        }

        public List<string> getCategories()
        {
            List<string> targetArea = new List<string>();

            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Playlists.db")))
                {
                    //find the user specified playlist and exercise
                   
                    var exerciseList = connection.Query<dbExercise>("SELECT * FROM dbExercise");

                    foreach(dbExercise name in exerciseList)
                    {
                        if(!targetArea.Contains(name.targetArea))
                            targetArea.Add(name.targetArea);
                    }

                    return targetArea;

                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);
                return null;
            }
        }

        public void loadData(SQLiteConnection connection)
        {
            try
            {
                using (connection)
                {
                    //insert playlists
                    var Day1 = new dbPlaylist { name = "Day 1", favorite = true };
                    connection.Insert(Day1);
                    var Day2 = new dbPlaylist { name = "Day 2", favorite = true };
                    connection.Insert(Day2);
                    var Day3 = new dbPlaylist { name = "Day 3", favorite = true };
                    connection.Insert(Day3);
                    var Day4 = new dbPlaylist { name = "Day 4" };
                    connection.Insert(Day4);
                    var Day5 = new dbPlaylist { name = "Day 5", favorite = true };
                    connection.Insert(Day5);

                    //The next 2 sets of Playlists will illustrate the multiple recyclerviews on the home page
                    //the playlist type is simply to easily pick them out of the database
                    //and show the potential for more sorting categories in the future
                    var chest = new dbPlaylist { name = "Chest", favorite = false, playlistType = "Upper Body" };
                    connection.Insert(chest);
                    var back = new dbPlaylist { name = "Back", favorite = false, playlistType = "Upper Body" };
                    connection.Insert(back);
                    var triceps = new dbPlaylist { name = "Triceps", favorite = false, playlistType = "Upper Body" };
                    connection.Insert(triceps);
                    var biceps = new dbPlaylist { name = "Biceps", favorite = false, playlistType = "Upper Body" };
                    connection.Insert(biceps);
                    var shoulders = new dbPlaylist { name = "Shoulders", favorite = false, playlistType = "Upper Body" };
                    connection.Insert(shoulders);

                    var glutes = new dbPlaylist { name = "Glutes", favorite = false, playlistType = "Lower Body" };
                    connection.Insert(glutes);
                    var calves = new dbPlaylist { name = "Calves", favorite = false, playlistType = "Lower Body" };
                    connection.Insert(calves);
                    var quads = new dbPlaylist { name = "Quads", favorite = false, playlistType = "Lower Body" };
                    connection.Insert(quads);
                    var hamstring = new dbPlaylist { name = "Hamstring", favorite = false, playlistType = "Lower Body" };
                    connection.Insert(hamstring);
                    var abs = new dbPlaylist { name = "Abs", favorite = false, playlistType = "Lower Body" };
                    connection.Insert(abs);


                    //insert exercises - chest
                    var benchPress = new dbExercise { name = "Bench Press", reps = "5", sets = "5", weight = "50", targetArea = "Chest", equipment = "Dumbbells" };
                    connection.Insert(benchPress);
                    var inclineBenchPress = new dbExercise { name = "Incline Bench Press", reps = "5", sets = "5", targetArea = "Chest", equipment = "Dumbbells", weight = "50" };
                    connection.Insert(inclineBenchPress);
                    var lyingFlying = new dbExercise { name = "Lying Fly", reps = "3", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Chest" };
                    connection.Insert(lyingFlying);
                    var aroundTheWorlds = new dbExercise { name = "Around The Worlds", reps = "5", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Chest" };
                    connection.Insert(aroundTheWorlds);
                    var inclinedFly = new dbExercise { name = "Incline Dumbbell Fly", reps = "8", sets = "2", equipment = "Dumbbells", weight = "50", targetArea = "Chest" };
                    connection.Insert(inclinedFly);

                    //insert exercises - arms
                    //triceps
                    var inclineHammerCurls = new dbExercise { name = "Incline Hammer Curls", reps = "3", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(inclineHammerCurls);
                    var concentrationCurls = new dbExercise { name = "Concentration Curls", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(concentrationCurls);
                    var bicepCurls = new dbExercise { name = "Bicep Curls", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(bicepCurls);
                    var xBodyHammerCurls = new dbExercise { name = "Cross Body Hammer Curls", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(xBodyHammerCurls);
                    var zottmanCurl = new dbExercise { name = "Zottman Curl", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(zottmanCurl);
                    var tricepKickback = new dbExercise { name = "Tricep Kickback", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(tricepKickback);
                    var dumbbellFloorPress = new dbExercise { name = "Dumbbell Floor Press", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(dumbbellFloorPress);
                    var weightedBenchDip = new dbExercise { name = "Weighted Bench Dip", reps = "8", sets = "3", equipment = "Bench", weight = "50", targetArea = "Arms" };
                    connection.Insert(weightedBenchDip);
                    var seatedTricepPress = new dbExercise { name = "Seated Tricep Press", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Arms" };
                    connection.Insert(seatedTricepPress);
                    var skullCrusher = new dbExercise { name = "Skull Crusher", reps = "8", sets = "3", equipment = "Barbell", weight = "50", targetArea = "Arms" };
                    connection.Insert(skullCrusher);

                    //insert exercises - back
                    var oneArmRow = new dbExercise { name = "One Arm Row", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Back" };
                    connection.Insert(oneArmRow);
                    var inclineRow = new dbExercise { name = "Incline Row", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Back" };
                    connection.Insert(inclineRow);
                    var bentOverRow = new dbExercise { name = "Bent Over Row", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Back" };
                    connection.Insert(bentOverRow);
                    var middleBackShrug = new dbExercise { name = "Middle Back Shrug", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Back" };
                    connection.Insert(middleBackShrug);
                    var manMaker = new dbExercise { name = "Man Maker", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Back" };
                    connection.Insert(manMaker);

                    //insert exercises - abs
                    var weightedCrunch = new dbExercise { name = "Weighted Crunch", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Abs" };
                    connection.Insert(weightedCrunch);
                    var weightedLegRaise = new dbExercise { name = "Weighted Leg Raise", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Abs" };
                    connection.Insert(weightedLegRaise);
                    var dumbbellSideBend = new dbExercise { name = "Dumbbell Side Bend", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Abs" };
                    connection.Insert(dumbbellSideBend);
                    var spellCaster = new dbExercise { name = "Spell Caster", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Abs" };
                    connection.Insert(spellCaster);
                    var vSitCrossJab = new dbExercise { name = "Dumbbell V-Sit Cross Jab", reps = "8", equipment = "Dumbbells", weight = "50", targetArea = "Abs" };
                    connection.Insert(vSitCrossJab);

                    //insert exercises - legs
                    //quads
                    var dumbbellSquat = new dbExercise { name = "Dumbbell Squat", reps = "8", sets = "3", equipment = "Dumbbells", weight = "250", targetArea = "Legs" };
                    connection.Insert(dumbbellSquat);
                    var lunges = new dbExercise { name = "Lunges", reps = "8", sets = "3", equipment = "Dumbbells", weight = "250", targetArea = "Legs" };
                    connection.Insert(lunges);                    
                    var stepUps = new dbExercise { name = "Step Ups", reps = "8", sets = "3", equipment = "Dumbbells", weight = "250", targetArea = "Legs" };
                    connection.Insert(stepUps);
                    var stiffLeggedDeadLift = new dbExercise { name = "Stiff-Legged DeadLift", reps = "8", sets = "3", equipment = "Barbell", weight = "250", targetArea = "Legs" };
                    connection.Insert(stiffLeggedDeadLift);
                    var powerSnatch = new dbExercise { name = "Power Snatch", reps = "8", sets = "3", equipment = "Barbell", weight = "250", targetArea = "Legs" };
                    connection.Insert(powerSnatch);

                    //hamstring
                    var cleanDeadlift = new dbExercise { name = "Clean Deadlift", reps = "8", sets = "3", equipment = "Barbell", weight = "250", targetArea = "Legs" };
                    connection.Insert(cleanDeadlift);
                    var sumoDeadlift = new dbExercise { name = "Sumo Deadlift", reps = "8", sets = "3", equipment = "Barbell", weight = "250", targetArea = "Legs" };
                    connection.Insert(sumoDeadlift);
                    var lyingLegCurls = new dbExercise { name = "Lying Leg Curls", reps = "8", sets = "3", equipment = "Machine", weight = "75", targetArea = "Legs" };
                    connection.Insert(lyingLegCurls);
                    var ballLegCurls = new dbExercise { name = "Ball Leg Curls", reps = "8", sets = "3", equipment = "Exercise Ball", weight = "250", targetArea = "Legs" };
                    connection.Insert(ballLegCurls);
                    var hangSnatch = new dbExercise { name = "Hang Snatch", reps = "8", sets = "3", equipment = "Barbell", weight = "250", targetArea = "Legs" };
                    connection.Insert(hangSnatch);


                    //glutes
                    var barbellGluteBridge = new dbExercise { name = "Barbell Glute Bridge", reps = "8", sets = "3", equipment = "Barbell", weight = "50", targetArea = "Legs" };
                    connection.Insert(barbellGluteBridge);
                    var barbellHipThrust = new dbExercise { name = "Barbell Hip Thrust", reps = "8", sets = "3", equipment = "Barbell", weight = "50", targetArea = "Legs" };
                    connection.Insert(barbellHipThrust);
                    var buttLift = new dbExercise { name = "Butt Lift", reps = "8", sets = "3", equipment = "Body Only",  targetArea = "Legs" };
                    connection.Insert(buttLift);
                    var flutterKicks = new dbExercise { name = "Flutter Kicks", reps = "8", sets = "3", equipment = "Body Only", targetArea = "Legs" };
                    connection.Insert(flutterKicks);
                    var singleLegFluteBridge = new dbExercise { name = "Single Leg Glute Bridge", reps = "8", sets = "3", equipment = "Body Only", targetArea = "Legs" };
                    connection.Insert(singleLegFluteBridge);

                    //calves
                    var standingCalfRaise = new dbExercise { name = "Standing Calf Raise", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Legs" };
                    connection.Insert(standingCalfRaise);
                    var seatedCalfRaise = new dbExercise { name = "Seated Calf Raise", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Legs" };
                    connection.Insert(seatedCalfRaise);
                    var calfPress = new dbExercise { name = "Calf Press", reps = "8", sets = "3", equipment = "Machine", weight = "50", targetArea = "Legs" };
                    connection.Insert(calfPress);
                    var balanceBoard = new dbExercise { name = "Balance Board", reps = "8", sets = "3", equipment = "Balance Board", targetArea = "Legs" };
                    connection.Insert(balanceBoard);
                    var ankleCircles = new dbExercise { name = "Ankle Circles", reps = "8", sets = "3", equipment = "Body Only", targetArea = "Legs" };
                    connection.Insert(ankleCircles);

                    //insert exercises - shoulders
                    var oneArmSideLats = new dbExercise { name = "One-Arm Side Laterals", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Shoulders" };
                    connection.Insert(oneArmSideLats);
                    var oneArmPress = new dbExercise { name = "One-Arm Dumbbell Press", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Shoulders" };
                    connection.Insert(oneArmPress);
                    var powerPartials = new dbExercise { name = "Power Partials", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Shoulders" };
                    connection.Insert(powerPartials);
                    var seatedPress = new dbExercise { name = "Seated Dumbbell Press", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Shoulders" };
                    connection.Insert(seatedPress);
                    var standingPress = new dbExercise { name = "Standing Dumbbell Press", reps = "8", sets = "3", equipment = "Dumbbells", weight = "50", targetArea = "Shoulders" };
                    connection.Insert(standingPress);

                    //make relationships
                    Day1.exercises = new List<dbExercise> { weightedCrunch, weightedLegRaise, benchPress, inclineBenchPress, lyingFlying };
                    connection.UpdateWithChildren(Day1);
                    Day2.exercises = new List<dbExercise> { benchPress, inclineBenchPress, lyingFlying };
                    connection.UpdateWithChildren(Day2);
                    Day3.exercises = new List<dbExercise> { dumbbellSquat, lunges, standingCalfRaise, oneArmSideLats, oneArmPress, seatedPress };
                    connection.UpdateWithChildren(Day3);
                    Day4.exercises = new List<dbExercise> { inclineRow, bentOverRow, middleBackShrug, inclineHammerCurls };
                    connection.UpdateWithChildren(Day4);
                    Day5.exercises = new List<dbExercise> { benchPress, inclineBenchPress };
                    connection.UpdateWithChildren(Day5);

                    //upper body
                    chest.exercises = new List<dbExercise> { benchPress, inclineBenchPress, lyingFlying, aroundTheWorlds, inclinedFly };
                    connection.UpdateWithChildren(chest);
                    back.exercises = new List<dbExercise> { oneArmRow, inclineRow, bentOverRow, middleBackShrug, manMaker };
                    connection.UpdateWithChildren(back);
                    triceps.exercises = new List<dbExercise> { tricepKickback, dumbbellFloorPress, weightedBenchDip, seatedTricepPress, skullCrusher };
                    connection.UpdateWithChildren(triceps);
                    biceps.exercises = new List<dbExercise> { inclineHammerCurls, concentrationCurls, bicepCurls, xBodyHammerCurls, zottmanCurl };
                    connection.UpdateWithChildren(biceps);
                    shoulders.exercises = new List<dbExercise> { oneArmSideLats, oneArmPress, powerPartials, seatedPress, standingPress };
                    connection.UpdateWithChildren(shoulders);

                    //lower body
                    glutes.exercises = new List<dbExercise> { barbellGluteBridge, barbellHipThrust, buttLift, flutterKicks, singleLegFluteBridge };
                    connection.UpdateWithChildren(glutes);
                    calves.exercises = new List<dbExercise> { standingCalfRaise, seatedCalfRaise, calfPress, balanceBoard, ankleCircles };
                    connection.UpdateWithChildren(calves);
                    quads.exercises = new List<dbExercise> { dumbbellSquat, lunges, stepUps, stiffLeggedDeadLift, powerSnatch };
                    connection.UpdateWithChildren(quads);
                    hamstring.exercises = new List<dbExercise> { cleanDeadlift, sumoDeadlift, lyingLegCurls, ballLegCurls, hangSnatch };
                    connection.UpdateWithChildren(hamstring);
                    abs.exercises = new List<dbExercise> { weightedCrunch, weightedLegRaise, dumbbellSideBend, spellCaster, vSitCrossJab };
                    connection.UpdateWithChildren(abs);                
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEX: ", ex.Message);                
            }
        }

        public void resetTables(SQLiteConnection connection)
        {
            connection.Execute("DELETE FROM dbPlaylist");
            connection.Execute("DELETE FROM dbExercise");
        }
    
    }
}

