/* eslint-disable ember/no-mixins */
import RESTAdapter from '@ember-data/adapter/rest';
import environment from 'poppin-ui/config/environment';
import { inject as service } from '@ember/service';
import { computed } from '@ember/object';

export default class ApplicationAdapter extends RESTAdapter {
	@service apiService;

  host = environment.hostURL;
	namespace = 'api';

	@computed('apiService.jwt')
	get headers() {
		return {
			Authorization: 'Bearer ' + this.apiService.jwt,
			'Content-Type': 'application/json',
			Accept: 'application/json, text/*, */*'
		};
	}
}
