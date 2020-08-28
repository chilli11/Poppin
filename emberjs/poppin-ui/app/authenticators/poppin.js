import { inject as service } from '@ember/service';
import Torii from 'ember-simple-auth/authenticators/torii';
import HttpResources from '../utils/http-resources';

export default class PoppinAuthenticator extends Torii {
	@service apiService;

	authenticate(credentials) {
		if (super.authenticate) super.authenticate(...arguments);

		return this.apiService.request({
			resource: HttpResources.login,
			body: credentials
		}).then(response => ({ access_token: response.token }));
	}
}