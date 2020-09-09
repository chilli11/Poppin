import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminRoute extends Route {
	@service accountService;
	beforeModel() {
		this.accountService.isAuthenticated()
			.then((response) => {
				if (response && !response.authorized) {
					return this.transitionTo('account');
				}
				return true;
			}).catch(() => this.transitionTo('account'));
	}
}
