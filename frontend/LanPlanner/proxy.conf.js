const token = process.env.DEV_ACCESS_TOKEN;

if (!token) {
  console.warn('DEV_ACCESS_TOKEN ist nicht gesetzt, die API wird 401 liefern.');
}

module.exports = {
  '/api': {
    target: 'http://localhost:5275',
    secure: false,
    changeOrigin: true,
    headers: token ? { 'Cf-Access-Jwt-Assertion': token } : {},
  },
};
