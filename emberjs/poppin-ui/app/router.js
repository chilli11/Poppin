import EmberRouter from '@ember/routing/router';
import config from './config/environment';

export default class Router extends EmberRouter {
  location = config.locationType;
  rootURL = config.rootURL;
}

Router.map(function() {
  this.route('admin', function() {
    this.route('locations', function() {
      this.route('location', { path: ':location_id' });
      this.route('add');
    });
  });
});
