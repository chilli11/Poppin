import Controller from '@ember/controller';
import { action } from '@ember/object'

export default class AccountMeController extends Controller {
	@action
	locationClickAction(location) {
		return this.transitionToRoute('locations.location', location);
	}
	
	@action
	vendorClickAction(vendor) {
		return this.transitionToRoute('vendors.vendor', vendor);
	}

	@action
	refreshTheRoute(profile) {
		const outModel = {
			profile: profile,
			vendors: this.model.vendors,
			favorites: this.model.favorites,
			hidden: this.model.hidden
		}
		return this.send('refreshRoute', outModel);
	}
}
