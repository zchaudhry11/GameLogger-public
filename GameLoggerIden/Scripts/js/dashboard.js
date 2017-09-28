$(document).ready(function () {
    initDashboard();
});

function initDashboard() {
    $.ajax({
        url: "/Dashboard/GetBacklogInfo",
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            console.log(res);
            var statistics = JSON.parse(res);

            var numBackloggedGames = statistics.num_backlogged_games;
            var finishBacklogTime = statistics.finish_backlog_time;
            var numCompleted = statistics.num_completed;
            var percCompleted = statistics.perc_completed;
            var currPlayingGame = statistics.curr_game_name;
            var currGameLength = statistics.curr_game_length;
            var currGameProgress = statistics.perc_completed_curr_game;
            var currGamePlayTime = statistics.curr_game_play_time;

            var numShortGames = statistics.num_short;
            var numMedGames = statistics.num_med;
            var numLongGames = statistics.num_long;

            var mostPlayedName = "";
            var mostPlayedLength = 0;

            var secondMostPlayedName = "";
            var secondMostPlayedLength = 0;

            var thirdMostPlayedName = "";
            var thirdMostPlayedLength = 0;

            // Value checks
            if (statistics.success == false) {
                numBackloggedGames = 0;
                finishBacklogTime = 0;
                numCompleted = 0;
                percCompleted = 0;
                currPlayingGame = "N/A";
                currGameLength = "N/A";
                currGameProgress = "N/A";
                numShortGames = 0;
                numMedGames = 0;
                numLongGames = 0;
                mostPlayedName = "N/A";
                mostPlayedLength = 0;
                secondMostPlayedName = "N/A";
                secondMostPlayedLength = 0;
                thirdMostPlayedName = "N/A";
                thirdMostPlayedLength = 0;
            }

            // Set most played game values
            if (statistics.most_played != null) {
                if (statistics.most_played[0] != null) {
                    mostPlayedName = statistics.most_played[0].name;
                    mostPlayedLength = statistics.most_played[0].play_time;
                }

                if (statistics.most_played[1] != null) {
                    secondMostPlayedName = statistics.most_played[1].name;
                    secondMostPlayedLength = statistics.most_played[1].play_time;
                }

                if (statistics.most_played[2] != null) {
                    thirdMostPlayedName = statistics.most_played[2].name;
                    thirdMostPlayedLength = statistics.most_played[2].play_time;
                }
            }

            // Update charts
            setChartTheme();

            $("#numBackloggedGames").html("Backlogged Games: <br />" + numBackloggedGames);
            $("#timeToFinish").html("It will take " + Math.ceil((finishBacklogTime / 24)) + " days to finish your backlog.");
            $("#completedGames").html("Completed Games: <br />" + numCompleted);
            $("#mostPlayedGameText").html("Most Played Game: <br /><br /><strong>" + mostPlayedName + "</strong><br /><br />Total Playtime: <br /><br />" + Math.round((mostPlayedLength / 60)) + " hours");

            if (statistics.success == false) {
                $("#percCompleted").parent().remove();
            }
            else {
                // Backlog completed percentage
                Highcharts.chart('percCompleted', {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: 0,
                        plotShadow: false
                    },
                    title: {
                        text: '%<br>of<br>Backlog<br>Completed',
                        align: 'center',
                        verticalAlign: 'middle',
                        y: 40
                    },
                    tooltip: {
                        pointFormat: '{series.name}: <b>{point.percentage:.0f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            size: '100%',
                            dataLabels: {
                                enabled: true,
                                distance: -50,
                                style: {
                                    fontWeight: 'bold',
                                    color: 'white'
                                },
                                format: '{y:.0f} %'
                            },
                            startAngle: -90,
                            endAngle: 90,
                            center: ['50%', '75%']
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: '% of Backlog',
                        innerSize: '50%',
                        data: [
                            ['Completed', percCompleted],
                            ['Uncompleted', (100 - percCompleted)]
                        ]
                    }]
                });
            }

            // Currently played game breakdown
            $("#currPlaying").html("Currently Playing: <br /><br /><strong>" + currPlayingGame + "</strong>");
            $("#currPlayingLength").html("Game Length: <br />" + currGameLength + " Hours");
            $("#currPlayTime").html("Current Playtime: <br />" + currGamePlayTime + " Hours");
            $("#currPlayingPercCompleted").html("Percent Complete: <br />" + currGameProgress + "%");

            if (currPlayingGame == "") {
                $("#currPlaying").html("Currently Playing: <br /><br /><strong>" + "N/A" + "</strong>");
                $("#currPlayingLength").html("Game Length: <br />" + "N/A");
                $("#currPlayTime").html("Current Playtime: <br />" + "N/A");
                $("#currPlayingPercCompleted").html("Percent Complete: <br />" + "N/A");
            }

            if (statistics.success == false) {
                $("#mostPlayedGameText").html("Most Played Game: <br /><br /><strong>" + mostPlayedName + "</strong><br /><br />Total Playtime: <br /><br />" + "N/A");
                $("#lengthBreakdown").parent().remove();
            }
            else {
                // Game length breakdown
                Highcharts.chart('lengthBreakdown', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: 'Game Length Breakdown'
                    },
                    legend: {
                        enabled: false
                    },
                    xAxis: {
                        categories: ['<8 hours', '8-40 hours', '>40 hours'],
                        labels: {
                            x: -10
                        }
                    },
                    yAxis: {
                        allowDecimals: false,
                        title: {
                            text: 'Total Games in Backlog'
                        }
                    },
                    series: [{
                        name: '<8 hours',
                        data: [numShortGames, 0, 0]
                    }, {
                        name: '8-40 hours',
                        data: [0, numMedGames, 0]
                    }, {
                        name: '>40 hours',
                        data: [0, 0, numLongGames]
                    }],
                    responsive: {
                        rules: [{
                            condition: {
                                maxWidth: 500
                            },
                            chartOptions: {
                                legend: {
                                    align: 'center',
                                    verticalAlign: 'bottom',
                                    layout: 'horizontal'
                                },
                                yAxis: {
                                    labels: {
                                        align: 'left',
                                        x: 0,
                                        y: -5
                                    },
                                    title: {
                                        text: null
                                    }
                                },
                                subtitle: {
                                    text: null
                                },
                                credits: {
                                    enabled: false
                                }
                            }
                        }]
                    }
                });
            }

            // Fix empty chart display bug
            if (mostPlayedLength <= 0) {
                mostPlayedLength = 1;
            }

            if (mostPlayedName == "N/A") {
                $("#mostPlayed").parent().remove();
            }
            else {
                // Most played games breakdown
                Highcharts.chart('mostPlayed', {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie'
                    },
                    title: {
                        text: 'Most Played Games Breakdown'
                    },
                    tooltip: {
                        pointFormat: '{series.name}: <b>{point.percentage:.0f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            size: '75%',
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false,
                                format: '{point.percentage:.1f} %',
                                style: {
                                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                                }
                            }
                        }
                    },
                    series: [{
                        name: '% of Playtime',
                        data: [
                            { name: mostPlayedName, y: mostPlayedLength },
                            { name: secondMostPlayedName, y: secondMostPlayedLength },
                            { name: thirdMostPlayedName, y: thirdMostPlayedLength }
                        ]
                    }]
                });
            }
        },
        error: function (res) {
            console.log(res);
        }
    });
}

// Update chart color scheme
function setChartTheme() {
    Highcharts.createElement('link', {
        href: 'https://fonts.googleapis.com/css?family=Unica+One',
        rel: 'stylesheet',
        type: 'text/css'
    }, null, document.getElementsByTagName('head')[0]);

    Highcharts.theme = {
        colors: ['#2b908f', '#90ee7e', '#f45b5b', '#7798BF', '#aaeeee', '#ff0066', '#eeaaee',
            '#55BF3B', '#DF5353', '#7798BF', '#aaeeee'],
        chart: {
            backgroundColor: {
                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 0 },
                stops: [
                    [0, '#B388FF'],
                    [1, '#B388FF']
                ]
            },
            style: {
                fontFamily: '\'Unica One\', sans-serif'
            },
            plotBorderColor: '#606063'
        },
        title: {
            style: {
                color: '#E0E0E3',
                textTransform: 'uppercase',
                fontSize: '20px'
            }
        },
        subtitle: {
            style: {
                color: '#E0E0E3',
                textTransform: 'uppercase'
            }
        },
        xAxis: {
            gridLineColor: '#707073',
            labels: {
                style: {
                    color: '#E0E0E3'
                }
            },
            lineColor: '#707073',
            minorGridLineColor: '#505053',
            tickColor: '#707073',
            title: {
                style: {
                    color: '#A0A0A3'

                }
            }
        },
        yAxis: {
            gridLineColor: '#707073',
            labels: {
                style: {
                    color: '#E0E0E3'
                }
            },
            lineColor: '#707073',
            minorGridLineColor: '#505053',
            tickColor: '#707073',
            tickWidth: 1,
            title: {
                style: {
                    color: '#A0A0A3'
                }
            }
        },
        tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.85)',
            style: {
                color: '#F0F0F0'
            }
        },
        plotOptions: {
            series: {
                dataLabels: {
                    color: '#B0B0B3'
                },
                marker: {
                    lineColor: '#333'
                }
            },
            boxplot: {
                fillColor: '#505053'
            },
            candlestick: {
                lineColor: 'white'
            },
            errorbar: {
                color: 'white'
            }
        },
        legend: {
            itemStyle: {
                color: '#E0E0E3'
            },
            itemHoverStyle: {
                color: '#FFF'
            },
            itemHiddenStyle: {
                color: '#606063'
            }
        },
        credits: {
            style: {
                color: '#666'
            }
        },
        labels: {
            style: {
                color: '#707073'
            }
        },

        drilldown: {
            activeAxisLabelStyle: {
                color: '#F0F0F3'
            },
            activeDataLabelStyle: {
                color: '#F0F0F3'
            }
        },

        navigation: {
            buttonOptions: {
                symbolStroke: '#DDDDDD',
                theme: {
                    fill: '#505053'
                }
            }
        },

        // scroll charts
        rangeSelector: {
            buttonTheme: {
                fill: '#505053',
                stroke: '#000000',
                style: {
                    color: '#CCC'
                },
                states: {
                    hover: {
                        fill: '#707073',
                        stroke: '#000000',
                        style: {
                            color: 'white'
                        }
                    },
                    select: {
                        fill: '#000003',
                        stroke: '#000000',
                        style: {
                            color: 'white'
                        }
                    }
                }
            },
            inputBoxBorderColor: '#505053',
            inputStyle: {
                backgroundColor: '#333',
                color: 'silver'
            },
            labelStyle: {
                color: 'silver'
            }
        },

        navigator: {
            handles: {
                backgroundColor: '#666',
                borderColor: '#AAA'
            },
            outlineColor: '#CCC',
            maskFill: 'rgba(255,255,255,0.1)',
            series: {
                color: '#7798BF',
                lineColor: '#A6C7ED'
            },
            xAxis: {
                gridLineColor: '#505053'
            }
        },

        scrollbar: {
            barBackgroundColor: '#808083',
            barBorderColor: '#808083',
            buttonArrowColor: '#CCC',
            buttonBackgroundColor: '#606063',
            buttonBorderColor: '#606063',
            rifleColor: '#FFF',
            trackBackgroundColor: '#404043',
            trackBorderColor: '#404043'
        },

        // special colors for some of the
        legendBackgroundColor: 'rgba(0, 0, 0, 0.5)',
        background2: '#505053',
        dataLabelsColor: '#B0B0B3',
        textColor: '#C0C0C0',
        contrastTextColor: '#F0F0F3',
        maskColor: 'rgba(255,255,255,0.3)'
    };

    // Apply the theme
    Highcharts.setOptions(Highcharts.theme);
}