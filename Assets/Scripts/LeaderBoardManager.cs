using UnityEngine;
// using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class LeaderBoardManager : MonoBehaviour
{
    void Start()
    {

    }

    void SaveScore()
    {
        // đăng một điểm số lên một bảng xếp hạng
        // Social.ReportScore(12345, "Cfji293fjsie_QA", (bool success) =>
        // {
        // });


        // Để đăng một điểm số có chứa một thẻ siêu dữ liệu, hãy sử dụng trực tiếp thực thể
        // PlayGamesPlatform.Instance.ReportScore(12345, "Cfji293fjsie_QA", "FirstDaily", (bool success) => {
        // });

        // Để hiển thị giao diện người dùng tích hợp sẵn cho tất cả các bảng xếp hạng, hãy gọi
        // Social.ShowLeaderboardUI();

        // PlayGamesPlatform.Instance.ShowLeaderboardUI("Cfji293fjsie_QA");



        // ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        // lb.id = "MY_LEADERBOARD_ID";
        // lb.LoadScores(ok =>
        //     {
        //         if (ok) {
        //             LoadUsersAndDisplay(lb);
        //         }
        //         else {
        //             Debug.Log("Error retrieving leaderboardi");
        //         }
        //     });


        // PlayGamesPlatform.Instance.LoadScores(
        //     GPGSIds.leaderboard_leaders_in_smoketesting,
        //     LeaderboardStart.PlayerCentered,
        //     100,
        //     LeaderboardCollection.Public,
        //     LeaderboardTimeSpan.AllTime,
        //     (data) =>
        //     {
        //         mStatus = "Leaderboard data valid: " + data.Valid;
        //         mStatus += "\n approx:" +data.ApproximateCount + " have " + data.Scores.Length;
        //     });
    }

    // internal void LoadUsersAndDisplay(ILeaderboard lb)
    // {
    //     // Get the user ids
    //     List<string> userIds = new List<string>();

    //     foreach(IScore score in lb.scores) {
    //         userIds.Add(score.userID);
    //     }
    //     // Load the profiles and display (or in this case, log)
    //     Social.LoadUsers(userIds.ToArray(), (users) =>
    //         {
    //             string status = "Leaderboard loading: " + lb.title + " count = " +
    //                 lb.scores.Length;
    //             foreach(IScore score in lb.scores) {
    //                 IUserProfile user = FindUser(users, score.userID);
    //                 status += "\n" + score.formattedValue + " by " +
    //                     (string)(
    //                         (user != null) ? user.userName : "**unk_" + score.userID + "**");
    //             }
    //             Debug.log(status);
    //         });
    // }
}
