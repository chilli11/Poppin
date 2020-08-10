import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';

export default class AccountService extends Service {
	@injectService apiService;

	registerAccount(credentials) {
		return this.apiService.request({
			resource: HttpResources.registerAccount,
			body: credentials
		});
	}
}
