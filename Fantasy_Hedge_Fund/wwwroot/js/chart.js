gradientChartOptionsConfigurationWithTooltipPurple = {
  maintainAspectRatio: false,
  legend: {
    display: false
  },

  tooltips: {
    backgroundColor: '#f5f5f5',
    titleFontColor: '#333',
    bodyFontColor: '#666',
    bodySpacing: 4,
    xPadding: 12,
    mode: "nearest",
    intersect: 0,
    position: "nearest"
  },
  responsive: true,
  scales: {
    yAxes: [{
      barPercentage: 1.6,
      gridLines: {
        drawBorder: false,
        color: 'rgba(29,140,248,0.0)',
        zeroLineColor: "transparent",
      },
      ticks: {
        suggestedMin: 60,
        suggestedMax: 125,
        padding: 20,
        fontColor: "#9a9a9a"
      }
    }],

    xAxes: [{
      barPercentage: 1.6,
      gridLines: {
        drawBorder: false,
        color: 'rgba(0,242,195,0.1)',
        zeroLineColor: "transparent",
      },
      ticks: {
        padding: 20,
        fontColor: "#9e9e9e"
      }
    }]
  }
};

var ctx = document.getElementById("chartLinePurple").getContext("2d");

var gradientStroke = ctx.createLinearGradient(0, 230, 0, 50);

gradientStroke.addColorStop(1, 'rgba(66,134,121,0.15)');
gradientStroke.addColorStop(0.2, 'rgba(66,134,121,0.0)');
gradientStroke.addColorStop(0, 'rgba(66,134,121,0)'); //purple colors

var data = {
labels: ['JAN','FEB','MAR','APR','MAY','JUN','JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'],
datasets: [{
label: "Data",
fill: true,
backgroundColor: gradientStroke,
borderColor: '#00d6b4',
borderWidth: 2,
borderDash: [],
borderDashOffset: 0.0,
pointBackgroundColor: '#00d6b4',
pointBorderColor: 'rgba(255,255,255,0)',
pointHoverBackgroundColor: '#00d6b4',
pointBorderWidth: 20,
pointHoverRadius: 4,
pointHoverBorderWidth: 15,
pointRadius: 4,
data: [120, 100, 70, 80, 120, 80,80, 100, 70, 80, 120, 80],
}]
};

var myChart = new Chart(ctx, {
  type: 'line',
  data: data,
  options: gradientChartOptionsConfigurationWithTooltipPurple
});