import { moment } from '../shared/moment';

export const ChartOptions = {
  responsive: true,
  legend: {
    display: true,
    position: 'top',
    labels: {
      filter: (legendItem, chartData) => {
        return legendItem.text.includes('Popularity');
      }
    }
  },
  scales: {
    xAxes: [{
      scaleLabel: {
        display: true,
        labelString: 'Date'
      },
      type: 'time',
      time: {
        unit: 'month'
      },
      distribution: 'linear'
    }],
    yAxes: [{
      scaleLabel: {
        display: true,
        labelString: 'Popularity %'
      }
    }]
  },
  tooltips: {
    callbacks: {
      label: (tooltipItem, data) => {
        const item = data.datasets[tooltipItem.datasetIndex];
        if (item.isGig) {
          return item.label;
        }

        if (item.isTwitter) {
          return `Number of tweets: ${item.data[tooltipItem.index].tweetCount}`;
        }

        return `${item.label}: ${Math.floor(tooltipItem.yLabel)}%`;
      },
      title: (tooltipItem, data, c) => {
        return moment(tooltipItem[0].xLabel).format('dddd Do MMM YYYY');
      }
    }
  },
  elements: {
    line: {
      tension: 0.1
    }
  },
  onClick: null
};

export const ChartColors = [
  {
    backgroundColor: 'rgba(0,0,0,0.0)',
    borderColor: 'rgba(29,185,84,1)',
    pointBackgroundColor: 'rgba(29,185,84,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
  },
  {
    backgroundColor: 'rgba(0,0,0,0.0)',
    borderColor: 'rgba(29,161,242,1)',
    pointBackgroundColor: 'rgba(29,161,242,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
  }
];
