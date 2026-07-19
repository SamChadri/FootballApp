using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Football.Core;

namespace Football.Data.Sqlite;

public static class DatabaseSeeder
{
    private static readonly string[] Filenames = new[]
    {
        "jersey_number_0_mac_resetich.webp",
        "jersey_number_0_nas_rankin.webp",
        "jersey_number_1_jayshon_platt.webp",
        "jersey_number_1_xavier_scott.webp",
        "jersey_number_2_carson_boyd.webp",
        "jersey_number_2_rj_jones_iii.webp",
        "jersey_number_3_kenyon_alston.webp",
        "jersey_number_3_nick_hankins_jr..webp",
        "jersey_number_4_daniel_brown.webp",
        "jersey_number_4_katin_houser.webp",
        "jersey_number_5_calil_valentine.webp",
        "jersey_number_5_jakwon_morris.webp",
        "jersey_number_6_jershaun_newton.webp",
        "jersey_number_6_juice_clarke.webp",
        "jersey_number_7_matthew_bailey.webp",
        "jersey_number_7_michael_clayton_ii.webp",
        "jersey_number_8_cameron_thomas.webp",
        "jersey_number_8_davon_grant.webp",
        "jersey_number_9_alex_perry.webp",
        "jersey_number_9_lavon_williams.webp",
        "jersey_number_10_james_kreutz.webp",
        "jersey_number_10_ty_robinson.webp",
        "jersey_number_11_brayden_trimble.webp",
        "jersey_number_11_jacob_alexander.webp",
        "jersey_number_12_dane_eisenmenger.webp",
        "jersey_number_12_isaiah_williams.webp",
        "jersey_number_13_hudson_clement.webp",
        "jersey_number_13_xanai_scott.webp",
        "jersey_number_14_andre_lovett_jr..webp",
        "jersey_number_14_maurice_smith_jr..webp",
        "jersey_number_15_kingston_shaw.webp",
        "jersey_number_16_logan_farrell.webp",
        "jersey_number_16_tanner_heckel.webp",
        "jersey_number_17_collin_dixon.webp",
        "jersey_number_17_isaiah_thomison.webp",
        "jersey_number_18_jack_gray.webp",
        "jersey_number_18_mason_muragin.webp",
        "jersey_number_19_jacob_eberhart.webp",
        "jersey_number_19_nate_cichy.webp",
        "jersey_number_20_almirian_thomas.webp",
        "jersey_number_20_john_forster.webp",
        "jersey_number_21_aidan_laughery.webp",
        "jersey_number_21_ben_clawson.webp",
        "jersey_number_22_cedric_wyche_ii.webp",
        "jersey_number_22_deuce_fillmore.webp",
        "jersey_number_23_jaylen_stewart.webp",
        "jersey_number_23_jordan_anderson.webp",
        "jersey_number_24_javari_barnett.webp",
        "jersey_number_24_will_holmes.webp",
        "jersey_number_25_des_straughton.webp",
        "jersey_number_25_nelsyn_wheeler.webp",
        "jersey_number_26_ismael_kante.webp",
        "jersey_number_27_corey_rashad.webp",
        "jersey_number_27_lucas_osada.webp",
        "jersey_number_28_karson_kaiser.webp",
        "jersey_number_28_kayden_bennett.webp",
        "jersey_number_29_james_finley.webp",
        "jersey_number_30_tony_williams.webp",
        "jersey_number_31_murphy_clement.webp",
        "jersey_number_32_tywan_cox.webp",
        "jersey_number_33_grant_beerman.webp",
        "jersey_number_34_erik_gayle.webp",
        "jersey_number_35_kaedyn_cobbs.webp",
        "jersey_number_37_caden_considine.webp",
        "jersey_number_37_ethan_moczulski.webp",
        "jersey_number_38_jack_paris.webp",
        "jersey_number_43_joe_barna.webp",
        "jersey_number_44_patrick_mahoney_iii.webp",
        "jersey_number_45_robert_edmonson.webp",
        "jersey_number_48_carter_smith.webp",
        "jersey_number_49_zach_haber.webp",
        "jersey_number_51_alfred_washington.webp",
        "jersey_number_51_parker_crim.webp",
        "jersey_number_52_champ_smith.webp",
        "jersey_number_52_pat_farrell.webp",
        "jersey_number_53_kai_pritchard.webp",
        "jersey_number_54_demetrius_john.webp",
        "jersey_number_54_griffin_rousseau.webp",
        "jersey_number_55_tj_mcmillen.webp",
        "jersey_number_55_tony_balanganayi.webp",
        "jersey_number_56_guillermo_gallardo.webp",
        "jersey_number_56_jj_hirdes.webp",
        "jersey_number_57_zach_barrett.webp",
        "jersey_number_58_landen_von_seggern.webp",
        "jersey_number_59_jake_renfro.webp",
        "jersey_number_62_casey_thomann.webp",
        "jersey_number_66_brandon_hansen.webp",
        "jersey_number_68_michael_mcdonough.webp",
        "jersey_number_6_jershaun_newton.webp",
        "jersey_number_6_juice_clarke.webp",
        "jersey_number_70_maika_matelau.webp",
        "jersey_number_71_nathan_knapik.webp",
        "jersey_number_72_christian_martin.webp",
        "jersey_number_74_zafir_stewart.webp",
        "jersey_number_75_brandon_henderson.webp",
        "jersey_number_76_dylan_frechette.webp",
        "jersey_number_78_eddie_tuerk.webp",
        "jersey_number_79_tj_taylor.webp",
        "jersey_number_80_eddie_kasper.webp",
        "jersey_number_81_lars_rau.webp",
        "jersey_number_81_leslie_mosley_jr..webp",
        "jersey_number_82_jacob_harvey.webp",
        "jersey_number_83_kaden_feagin.webp",
        "jersey_number_84_grant_smith.webp",
        "jersey_number_85_will_vala_iii.webp",
        "jersey_number_86_christian_abney.webp",
        "jersey_number_87_bj_thurman.webp",
        "jersey_number_88_davin_stoffel.webp",
        "jersey_number_89_tanner_hollinger.webp",
        "jersey_number_90_carter_janki.webp",
        "jersey_number_91_cameron_brooks.webp",
        "jersey_number_92_deon_williams.webp",
        "jersey_number_93_ean_rhea.webp",
        "jersey_number_94_connor_sullivan.webp",
        "jersey_number_95_isaiah_white.webp",
        "jersey_number_96_darrell_prater.webp",
        "jersey_number_98_joshua_davis.webp",
        "jersey_number_99_king_liggins.webp"
    };

    public static async Task SeedAsync(IFootballRepository repo)
    {
        await repo.InitializeAsync();

        // Wipe database if generic "Player 1" seed exists, or if empty
        try
        {
            var existingPlayers = await repo.GetPlayersAsync();
            if (existingPlayers.Any(p => p.Name.StartsWith("Player ")))
            {
                if (repo is SqliteFootballRepository sqlRepo)
                {
                    await sqlRepo.DropAllTablesAsync();
                    await repo.InitializeAsync();
                }
            }
            else if (existingPlayers.Count > 0)
            {
                // Already seeded with real players
                return;
            }
        }
        catch
        {
            if (repo is SqliteFootballRepository sqlRepo)
            {
                await sqlRepo.DropAllTablesAsync();
                await repo.InitializeAsync();
            }
        }

        // Create 3 seasons
        var seasons = new[] { 1, 2, 3 };
        var years = new[] { 2021, 2022, 2023 };

        int playerId = 1;
        int gameId = 1;
        int playId = 1;
        var rand = new Random(42);
        var yearClasses = new[] { "FR", "SO", "JR", "SR" };

        var pattern = new Regex(@"^jersey_number_(\d+)_(.+)\.webp$", RegexOptions.IgnoreCase);

        for (int i = 0; i < seasons.Length; i++)
        {
            var seasonId = seasons[i];
            var year = years[i];

            // Add Games
            var seasonGameIds = new List<int>();
            for (int g = 1; g <= 3; g++)
            {
                var game = new Game(gameId, g, new DateTime(year, 9, g * 7), $"Opponent {g}", 'W', 24, 10, "H", seasonId);
                await repo.AddGameAsync(game);
                seasonGameIds.Add(gameId);
                gameId++;
            }

            // Parse and Add Players
            var currentSeasonPlayers = new List<Player>();

            foreach (var filename in Filenames)
            {
                var match = pattern.Match(filename);
                if (!match.Success) continue;

                int jerseyNum = int.Parse(match.Groups[1].Value);
                string rawName = match.Groups[2].Value
                    .Replace("_", " ");

                // Title Case the player name
                string displayName = System.Globalization.CultureInfo.CurrentCulture
                    .TextInfo.ToTitleCase(rawName);

                string position = AssignPosition(jerseyNum);
                string playerYear = yearClasses[rand.Next(yearClasses.Length)];

                var p = new Player(
                    Id: playerId,
                    Number: jerseyNum,
                    Name: displayName,
                    Position: position,
                    Year: playerYear,
                    TeamId: 1,
                    SeasonId: seasonId);

                await repo.AddPlayerAsync(p);
                currentSeasonPlayers.Add(p);
                playerId++;
            }

            // Add Plays
            foreach (var gId in seasonGameIds)
            {
                foreach (var p in currentSeasonPlayers)
                {
                    // Generate a few plays per player per game (1 to 4)
                    int numPlays = rand.Next(1, 5);
                    for (int pl = 0; pl < numPlays; pl++)
                    {
                        var play = new Play(
                            Id: playId,
                            PlayNum: pl + 1,
                            Calls: "BASE",
                            PlayerId: p.Id,
                            NumPenalties: rand.Next(0, 2),
                            PenaltyNames: "",
                            PlayYards: rand.Next(-2, 15),
                            Tackles: rand.Next(0, 3),
                            Tech: 1, Purs: 1, Mtp: 1,
                            Type: 'R',
                            Stat1: "", Stat2: "",
                            Loaf: false,
                            Comment: "",
                            Position: p.Position,
                            GameId: gId,
                            TeamId: 1,
                            SeasonId: seasonId);

                        await repo.AddPlayAsync(play);
                        playId++;
                    }
                }
            }
        }
    }

    private static string AssignPosition(int num) => num switch
    {
        >= 0 and <= 0   => "QB",
        >= 1 and <= 9   => (num % 3) switch { 0 => "QB", 1 => "WR", _ => "CB" },
        >= 10 and <= 19 => (num % 3) switch { 0 => "WR", 1 => "QB", _ => "CB" },
        >= 20 and <= 29 => (num % 2) switch { 0 => "HB", _ => "CB" },
        >= 30 and <= 39 => (num % 2) switch { 0 => "HB", _ => "FS" },
        >= 40 and <= 49 => (num % 2) switch { 0 => "FB", _ => "MLB" },
        >= 50 and <= 59 => (num % 2) switch { 0 => "C",  _ => "OLB" },
        >= 60 and <= 69 => "G",
        >= 70 and <= 79 => "T",
        >= 80 and <= 89 => (num % 2) switch { 0 => "WR", _ => "TE" },
        >= 90 and <= 99 => (num % 2) switch { 0 => "DT", _ => "DE" },
        _ => "WR"
    };
}

