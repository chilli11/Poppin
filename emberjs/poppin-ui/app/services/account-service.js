import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';

export default class AccountService extends Service {
	@injectService apiService;
	@injectService session;

	registerAccount(credentials) {
		return this.apiService.request({
			resource: HttpResources.registerAccount,
			body: credentials
		});
	}

	login(credentials) {
		return this.apiService.request({
			resource: HttpResources.login,
			body: credentials
		}).then(response => this.apiService.jwt = response.token);
	}
}
