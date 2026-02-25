import { CanDeactivateFn } from '@angular/router';
import { MemberProfile } from '../../features/member-profile/member-profile';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberProfile> = (component) => {
  if (component.memberProfileEditForm?.dirty) {
    return confirm('You have unsaved changes. Do you really want to leave?');
  }
  return true;
};
