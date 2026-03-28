import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Rate } from 'k6/metrics';

const orderCreated = new Counter('orders_created_total');
const errorRate = new Rate('error_rate');

export const options = {
  stages: [
    { duration: '30s', target: 100 },
    { duration: '1m', target: 100 },
    { duration: '30s', target: 0 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
    http_req_duration: ['p(99)<1000'],
    error_rate: ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL;
const ENDPOINT = `${BASE_URL}/v1/orders`;

const PRODUCTS = [
  'product-001',
  'product-002',
  'product-003',
  'product-004',
  'product-005',
];

function randomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomPrice() {
  return parseFloat((Math.random() * 500 + 1).toFixed(2));
}

function buildPayload() {
  const itemCount = randomInt(1, 5);
  const items = [];

  for (let i = 0; i < itemCount; i++) {
    items.push({
      productId: PRODUCTS[randomInt(0, PRODUCTS.length - 1)],
      quantity: randomInt(1, 10),
      unitPrice: randomPrice(),
    });
  }

  return {
    customerId: `customer-${randomInt(1, 10000)}`,
    items,
  };
}

export default function () {
  const payload = JSON.stringify(buildPayload());
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
    timeout: '10s',
  };

  const res = http.post(ENDPOINT, payload, params);

  const success = check(res, {
    'status is 202': (r) => r.status === 202,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });

  errorRate.add(!success);

  if (res.status === 202) {
    orderCreated.add(1);
  }

  sleep(randomInt(0, 1));
}