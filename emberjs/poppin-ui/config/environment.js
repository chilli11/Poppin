'use strict';

module.exports = function(environment) {
  let ENV = {
    modulePrefix: 'poppin-ui',
    podModulePrefix: 'poppin-ui/pods',
    environment,
		rootURL: '/',
		hostURL: '/',
		apiURL: '/api/',
		locationType: 'auto',
		
		'ember-simple-auth-token': {
			serverTokenEndpoint: '/api/identity/login',
			tokenPropertyName: 'token',
			authorizationHeaderName: 'Authorization',
			authorizationPrefix: 'Bearer '
		},

    EmberENV: {
      FEATURES: {
        // Here you can enable experimental features on an ember canary build
        // e.g. EMBER_NATIVE_DECORATOR_SUPPORT: true
      },
      EXTEND_PROTOTYPES: {
        // Prevent Ember Data from overriding Date.parse.
        Date: false
      }
    },

    APP: {
      // Here you can pass flags/options to your application instance
      // when it is created
    }
	};
	

  if (environment === 'development') {
    ENV.APP.LOG_RESOLVER = true;
    ENV.APP.LOG_ACTIVE_GENERATION = true;
    ENV.APP.LOG_TRANSITIONS = true;
    ENV.APP.LOG_TRANSITIONS_INTERNAL = true;
		ENV.APP.LOG_VIEW_LOOKUPS = true;
		
		// ENV.hostURL = 'https://localhost:44367';
		// ENV.apiURL = ENV.hostURL + '/api/';
  }

  if (environment === 'test') {
    // Testem prefers this...
    ENV.locationType = 'none';

    // keep test console output quieter
    ENV.APP.LOG_ACTIVE_GENERATION = false;
    ENV.APP.LOG_VIEW_LOOKUPS = false;

    ENV.APP.rootElement = '#ember-testing';
    ENV.APP.autoboot = false;
		ENV.apiURL = ENV.hostURL + '/api/';
  }

  if (environment === 'production') {
		// here you can enable a production-specific feature
		// ENV.hostURL = 'http://poppindev2-env.eba-zygausnr.us-west-2.elasticbeanstalk.com';
		// ENV.apiURL = ENV.hostURL + '/api/';
  }

  return ENV;
};
