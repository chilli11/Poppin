import EmberRouter from '@ember/routing/router';
import config from './config/environment';

export default class Router extends EmberRouter {
    location = config.locationType;
    rootURL = config.rootURL;
}

Router.map(function() {
    this.route('locations', function() {
        this.route('location', { path: ':location_id' });
        this.route('add');
    });
    this.route('vendors', function () {
      this.route('vendor', { path: ':vendor_id'});
      this.route('add');
      this.route('loading');
    });
  this.route('search', function() {});
  this.route('account', function() {
    this.route('index');
    this.route('register');
    this.route('login');
    this.route('me');
  });
});
