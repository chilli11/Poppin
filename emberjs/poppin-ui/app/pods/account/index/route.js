import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';
import HttpResources from '../../../utils/http-resources';

export default class AccountIndexRoute extends Route {
	@service apiService;
	model() {
		return this.apiService.request({
			resource: HttpResources.myAccount,
		}).then(({ user }) => user);
	}
}
