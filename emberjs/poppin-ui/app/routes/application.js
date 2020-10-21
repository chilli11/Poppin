import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminRoute extends Route {
	@service accountService;
	beforeModel() {
		const names = ['account.register', 'account.login', 'account.index'];
		this.accountService.isAuthenticated()
			.then((response) => {
				if (response && !response.authorized && names.indexOf(this.routeName) === -1) {
					return this.transitionTo('account.login');
				}
				return true;
			}).catch(() => names.indexOf(this.routeName) === -1 ? this.transitionTo('account.login') : true);
	}
}
